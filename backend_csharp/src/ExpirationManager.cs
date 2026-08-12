using GameEngine.Engine;

namespace GameEngine.Backend;

public sealed class ExpirationManager
{
    private readonly InMemoryGameStore _store;
    private readonly GameStatistics _stats;

    public ExpirationManager(InMemoryGameStore store, GameStatistics stats)
    {
        _store = store;
        _stats = stats;
    }

    public IReadOnlyList<string> Sweep()
    {
        var expired = _store.EvictExpired();
        RefreshStats();
        return expired;
    }

    public void RefreshStats()
    {
        _stats.SetActiveSessions(_store.Count);
        _stats.SetActivePlayers(_store.PlayerCount);
    }
}
