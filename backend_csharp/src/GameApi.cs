using GameEngine.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GameEngine.Backend;

public static class GameApi
{
    public static IEndpointRouteBuilder MapGameApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", (GameService svc) => Results.Json(svc.Health()));

        app.MapGet("/version", (VersionInfo version) => Results.Json(new
        {
            application = version.Application,
            frontend_dotnet = version.FrontendDotnet,
            backend_dotnet = version.BackendDotnet,
            branch = version.Branch,
            game_engine = version.GameEngine
        }));

        app.MapGet("/game/stats", (GameService svc) => Results.Json(svc.Stats()));

        app.MapPost("/game", async (GameRequest request, GameService svc) =>
        {
            var response = await svc.CreateAsync(request);
            return response.Success ? Results.Json(response) : Results.BadRequest(response);
        });

        app.MapPost("/game/{id}/join", async (string id, GameRequest request, GameService svc) =>
        {
            var response = await svc.JoinAsync(id, request);
            return response.Success ? Results.Json(response) : Results.BadRequest(response);
        });

        app.MapPost("/game/{id}/move", async (string id, GameRequest request, GameService svc) =>
        {
            var response = await svc.MoveAsync(id, request);
            return response.Success ? Results.Json(response) : Results.BadRequest(response);
        });

        app.MapGet("/game/{id}", (string id, GameService svc) =>
        {
            var response = svc.Get(id);
            return response.Success ? Results.Json(response) : Results.NotFound(response);
        });

        app.MapDelete("/game/{id}", async (string id, GameService svc) =>
        {
            var response = await svc.LeaveAsync(id);
            return Results.Json(response);
        });

        return app;
    }
}
