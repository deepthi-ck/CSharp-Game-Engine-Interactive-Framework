using System.Text.Json;
using GameEngine.Backend;
using GameEngine.Distribution;
using GameEngine.Engine;
using GameEngine.Shared;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var configuration = LoadConfiguration();
var policy = GamePolicy.FromConfiguration(configuration);
var store = new InMemoryGameStore();
var stats = new GameStatistics();
stats.SetNodeSlotCount(policy.SessionSlotCount);

var registry = new SessionRegistry();
for (var i = 0; i < policy.SessionSlotCount; i++)
    registry.Register(new GameSessionNode(i, store, policy));

var router = new SessionRouter(registry, policy.SessionSlotCount);
var sync = new SyncManager();
var eviction = new EvictionManager(store, policy, stats);
var expiration = new ExpirationManager(store, stats);
var manager = new GameManager(router, sync, stats, eviction, expiration, store);
var service = new GameService(manager);

var frontend = Environment.GetEnvironmentVariable("FRONTEND_DOTNET")
    ?? typeof(Program).Assembly.GetCustomAttributes(false)
        .OfType<System.Reflection.AssemblyMetadataAttribute>()
        .FirstOrDefault(a => a.Key == "FrontendDotnet")?.Value
    ?? DetectFromProps("FrontendDotnetVersion") ?? "6";
var backend = Environment.GetEnvironmentVariable("BACKEND_DOTNET")
    ?? DetectFromProps("BackendDotnetVersion")
    ?? ExtractTfmMajor()
    ?? "8";
var branch = Environment.GetEnvironmentVariable("BRANCH_NAME")
    ?? DetectFromProps("BranchName")
    ?? "CSharp_FE6_BE8";

var version = VersionInfo.FromEnvironment(frontend, backend, branch, "ready");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(configuration);
builder.Services.AddSingleton(policy);
builder.Services.AddSingleton(store);
builder.Services.AddSingleton(stats);
builder.Services.AddSingleton(registry);
builder.Services.AddSingleton(router);
builder.Services.AddSingleton(sync);
builder.Services.AddSingleton(eviction);
builder.Services.AddSingleton(expiration);
builder.Services.AddSingleton(manager);
builder.Services.AddSingleton(service);
builder.Services.AddSingleton(version);
builder.Services.AddSingleton<IGameBroadcaster, SignalRBroadcaster>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true)));
builder.Services.AddSignalR();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("csharp-game-engine"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddConsoleExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddConsoleExporter());

var app = builder.Build();
var broadcaster = app.Services.GetRequiredService<IGameBroadcaster>();
sync.SetBroadcaster(broadcaster);

app.UseCors();
app.MapGameApi();
app.MapHub<GameHub>(configuration.HubPath);
app.MapGet("/", () => Results.Json(new
{
    application = version.Application,
    branch = version.Branch,
    status = "running"
}));

if (string.Equals(Environment.GetEnvironmentVariable("SEED_SAMPLE_DATA"), "1", StringComparison.Ordinal))
    LoadSampleData(service);

var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? configuration.ApiBaseUrl;
app.Urls.Clear();
app.Urls.Add(urls);
Console.WriteLine($"Game platform listening on {urls} branch={branch} FE={frontend} BE={backend}");
app.Run();

static GameConfiguration LoadConfiguration()
{
    var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "config", "gamesettings.json");
    path = Path.GetFullPath(path);
    if (!File.Exists(path))
        path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "config", "gamesettings.json"));
    if (File.Exists(path))
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameConfiguration>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new GameConfiguration();
    }
    return new GameConfiguration();
}

static void LoadSampleData(GameService service)
{
    var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "data", "sample-game-data.json"));
    if (!File.Exists(path)) return;
    using var doc = JsonDocument.Parse(File.ReadAllText(path));
    if (!doc.RootElement.TryGetProperty("seed_games", out var games)) return;
    foreach (var g in games.EnumerateArray())
    {
        var id = g.GetProperty("gameId").GetString() ?? "game:1001";
        var owner = g.GetProperty("owner").GetString() ?? "Visvantha";
        try
        {
            service.CreateAsync(new GameRequest { GameId = id, Player = owner }).GetAwaiter().GetResult();
        }
        catch { /* already seeded */ }
    }
}

static string? DetectFromProps(string key)
{
    var props = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Directory.Build.props"));
    if (!File.Exists(props)) return null;
    var text = File.ReadAllText(props);
    var tag = $"<{key}>";
    var start = text.IndexOf(tag, StringComparison.Ordinal);
    if (start < 0) return null;
    start += tag.Length;
    var end = text.IndexOf('<', start);
    return end < 0 ? null : text[start..end].Trim();
}

static string? ExtractTfmMajor()
{
    var tfm = AppContext.TargetFrameworkName; // e.g. .NETCoreApp,Version=v8.0
    if (tfm is null) return null;
    var idx = tfm.LastIndexOf('v');
    if (idx < 0) return null;
    var ver = tfm[(idx + 1)..];
    return ver.Split('.')[0];
}

public partial class Program { }


