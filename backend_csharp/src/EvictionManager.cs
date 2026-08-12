using GameEngine.Engine;

namespace GameEngine.Backend;

public sealed class EvictionManager
{
    private readonly InMemoryGameStore _store;
    private readonly GamePolicy _policy;
    private readonly GameStatistics _stats;

    public EvictionManager(InMemoryGameStore store, GamePolicy policy, GameStatistics stats)
    {
        _store = store;
        _policy = policy;
        _stats = stats;
    }

    public string? EnsureCapacity()
    {
        if (_store.Count < _policy.MaxSessions)
            return null;

        var evicted = _store.EvictLeastRecentlyUsed();
        _stats.SetActiveSessions(_store.Count);
        _stats.SetActivePlayers(_store.PlayerCount);
        return evicted;
    }
}
