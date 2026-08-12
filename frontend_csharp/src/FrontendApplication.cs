using GameEngine.Frontend;
using GameEngine.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<GameDashboard>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"]
    ?? builder.HostEnvironment.BaseAddress.Replace("5173", "5080");
if (string.IsNullOrWhiteSpace(apiBase) || apiBase.Contains("localhost", StringComparison.OrdinalIgnoreCase) == false)
    apiBase = "http://localhost:5080/";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<GameClient>();

var frontend = Environment.GetEnvironmentVariable("FRONTEND_DOTNET") ?? Detect("FrontendDotnetVersion") ?? "6";
var backend = Environment.GetEnvironmentVariable("BACKEND_DOTNET") ?? Detect("BackendDotnetVersion") ?? "8";
var branch = Environment.GetEnvironmentVariable("BRANCH_NAME") ?? Detect("BranchName") ?? "CSharp_FE6_BE8";
builder.Services.AddSingleton(VersionInfo.FromEnvironment(frontend, backend, branch));

await builder.Build().RunAsync();

static string? Detect(string key)
{
    // Embedded at build via Directory.Build.props constants if present in wwwroot/appsettings
    return null;
}
