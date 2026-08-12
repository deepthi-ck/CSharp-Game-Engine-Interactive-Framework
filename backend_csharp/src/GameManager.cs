using GameEngine.Distribution;
using GameEngine.Engine;
using GameEngine.Shared;

namespace GameEngine.Backend;

public sealed class GameManager
{
    private readonly SessionRouter _router;
    private readonly SyncManager _sync;
    private readonly GameStatistics _stats;
    private readonly EvictionManager _eviction;
    private readonly ExpirationManager _expiration;
    private readonly InMemoryGameStore _store;

    public GameManager(
        SessionRouter router,
        SyncManager sync,
        GameStatistics stats,
        EvictionManager eviction,
        ExpirationManager expiration,
        InMemoryGameStore store)
    {
        _router = router;
        _sync = sync;
        _stats = stats;
        _eviction = eviction;
        _expiration = expiration;
        _store = store;
    }

    public InMemoryGameStore Store => _store;
    public GameStatistics Stats => _stats;
    public SyncManager Sync => _sync;
    public SessionRouter Router => _router;

    public async Task<GameResponse> CreateAsync(string gameId, string owner)
    {
        _expiration.Sweep();
        _eviction.EnsureCapacity();

        var session = new GameSession(_router.Route(gameId));
        var entry = session.Create(gameId, owner);
        _stats.RecordCreate();
        _expiration.RefreshStats();
        await _sync.PublishAsync(gameId, entry, "GameCreated");
        return Ok(entry, "CREATE = SUCCESS");
    }

    public async Task<GameResponse> JoinAsync(string gameId, string player)
    {
        _expiration.Sweep();
        var session = new GameSession(_router.Route(gameId));
        var entry = session.Join(gameId, player);
        _stats.RecordJoin();
        _expiration.RefreshStats();
        await _sync.PublishAsync(gameId, entry, "PlayerJoined");
        return Ok(entry, "JOIN = SUCCESS");
    }

    public async Task<GameResponse> MoveAsync(string gameId, string player, string move)
    {
        _expiration.Sweep();
        try
        {
            var session = new GameSession(_router.Route(gameId));
            var entry = session.Play(gameId, player, move);
            _stats.RecordMoveAccepted();
            await _sync.PublishAsync(gameId, entry, "MoveAccepted");
            return Ok(entry, "MOVE accepted");
        }
        catch (Exception ex)
        {
            _stats.RecordMoveRejected();
            return Fail(ex.Message);
        }
    }

    public GameResponse Get(string gameId)
    {
        _expiration.Sweep();
        var session = new GameSession(_router.Route(gameId));
        var entry = session.GetState(gameId);
        if (entry is null)
            return Fail("NOT_FOUND", "ENDED");
        return Ok(entry, "OK");
    }

    public async Task<GameResponse> LeaveAsync(string gameId, string? player = null)
    {
        var session = new GameSession(_router.Route(gameId));
        var existed = session.Leave(gameId, player);
        if (!existed)
            return Fail("NOT_FOUND", "ENDED");

        _stats.RecordLeave();
        _expiration.RefreshStats();
        await _sync.PublishEndedAsync(gameId);
        return new GameResponse { Success = true, Status = "ENDED", Message = "LEAVE/END = SUCCESS" };
    }

    public object Health()
    {
        _expiration.Sweep();
        return new
        {
            status = "healthy",
            game_engine = "available",
            sessions = _store.Count
        };
    }

    private static GameResponse Ok(GameEntry entry, string message) => new()
    {
        Success = true,
        Status = "OK",
        Message = message,
        Game = entry
    };

    private static GameResponse Fail(string message, string status = "ERROR") => new()
    {
        Success = false,
        Status = status,
        Message = message
    };
}
