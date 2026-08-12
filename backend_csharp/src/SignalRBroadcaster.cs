using GameEngine.Distribution;
using GameEngine.Shared;
using Microsoft.AspNetCore.SignalR;

namespace GameEngine.Backend;

public sealed class SignalRBroadcaster : IGameBroadcaster
{
    private readonly IHubContext<GameHub> _hub;

    public SignalRBroadcaster(IHubContext<GameHub> hub) => _hub = hub;

    public Task BroadcastGameUpdateAsync(string gameId, GameEntry entry, string eventName) =>
        _hub.Clients.Group(gameId).SendAsync(eventName, entry);

    public Task BroadcastGameEndedAsync(string gameId) =>
        _hub.Clients.Group(gameId).SendAsync("GameEnded", new { gameId, status = "ENDED" });
}
