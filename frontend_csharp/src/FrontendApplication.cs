using GameEngine.Frontend;
using GameEngine.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"]
    ?? builder.HostEnvironment.BaseAddress.Replace("5173", "5080");
if (string.IsNullOrWhiteSpace(apiBase) || apiBase.Contains("localhost", StringComparison.OrdinalIgnoreCase) == false)
    apiBase = "http://localhost:5080/";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<GameClient>();

var frontend = Environment.GetEnvironmentVariable("FRONTEND_DOTNET")
    ?? builder.Configuration["FrontendDotnet"]
    ?? "6";
var backend = Environment.GetEnvironmentVariable("BACKEND_DOTNET")
    ?? builder.Configuration["BackendDotnet"]
    ?? "8";
var branch = Environment.GetEnvironmentVariable("BRANCH_NAME")
    ?? builder.Configuration["BranchName"]
    ?? "CSharp_FE6_BE8";
builder.Services.AddSingleton(VersionInfo.FromEnvironment(frontend, backend, branch));

await builder.Build().RunAsync();
