using GameEngine.Shared;
using Microsoft.AspNetCore.SignalR;

namespace GameEngine.Backend;

public sealed class GameHub : Hub
{
    private readonly GameService _service;

    public GameHub(GameService service) => _service = service;

    public async Task JoinRoom(string gameId, string player)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        var response = await _service.JoinAsync(gameId, new GameRequest { Player = player });
        await Clients.Caller.SendAsync("JoinResult", response);
        if (response.Success && response.Game is not null)
            await Clients.Group(gameId).SendAsync("PlayerJoined", response.Game);
    }

    public async Task PlayMove(string gameId, string player, string move)
    {
        var response = await _service.MoveAsync(gameId, new GameRequest { Player = player, Move = move });
        await Clients.Caller.SendAsync("MoveResult", response);
        if (response.Success && response.Game is not null)
            await Clients.Group(gameId).SendAsync("GameUpdated", response.Game);
    }

    public async Task LeaveRoom(string gameId, string player)
    {
        var response = await _service.LeaveAsync(gameId, player);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerLeft", new { gameId, player, response.Status });
        await Clients.Caller.SendAsync("LeaveResult", response);
    }
}
