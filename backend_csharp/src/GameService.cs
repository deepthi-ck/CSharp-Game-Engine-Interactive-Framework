using GameEngine.Shared;

namespace GameEngine.Backend;

public sealed class GameService
{
    private readonly GameManager _manager;

    public GameService(GameManager manager) => _manager = manager;

    public Task<GameResponse> CreateAsync(GameRequest request)
    {
        var gameId = string.IsNullOrWhiteSpace(request.GameId) ? $"game:{Guid.NewGuid():N}" : request.GameId!;
        var owner = string.IsNullOrWhiteSpace(request.Player) ? "guest" : request.Player!;
        return _manager.CreateAsync(gameId, owner);
    }

    public Task<GameResponse> JoinAsync(string gameId, GameRequest request)
    {
        var player = string.IsNullOrWhiteSpace(request.Player) ? "guest" : request.Player!;
        return _manager.JoinAsync(gameId, player);
    }

    public Task<GameResponse> MoveAsync(string gameId, GameRequest request)
    {
        var player = string.IsNullOrWhiteSpace(request.Player) ? "guest" : request.Player!;
        var move = request.Move ?? request.Action ?? string.Empty;
        return _manager.MoveAsync(gameId, player, move);
    }

    public GameResponse Get(string gameId) => _manager.Get(gameId);

    public Task<GameResponse> LeaveAsync(string gameId, string? player = null) =>
        _manager.LeaveAsync(gameId, player);

    public object Stats() => _manager.Stats.Snapshot();
    public object Health() => _manager.Health();
    public GameManager Manager => _manager;
}
