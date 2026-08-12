using GameEngine.Distribution;
using GameEngine.Engine;

namespace GameEngine.Backend.Tests;

internal static class TestHost
{
    public static (GameService Service, GameManager Manager, SyncManager Sync, InMemoryGameStore Store) Create(int maxSessions = 32, int ttlSeconds = 300, int slots = 3)
    {
        var policy = new GamePolicy
        {
            MaxSessions = maxSessions,
            SessionTtlSeconds = ttlSeconds,
            MaxPlayersPerSession = 8,
            SessionSlotCount = slots
        };
        var store = new InMemoryGameStore();
        var stats = new GameStatistics();
        stats.SetNodeSlotCount(slots);
        var registry = new SessionRegistry();
        for (var i = 0; i < slots; i++)
            registry.Register(new GameSessionNode(i, store, policy));
        var router = new SessionRouter(registry, slots);
        var sync = new SyncManager();
        var eviction = new EvictionManager(store, policy, stats);
        var expiration = new ExpirationManager(store, stats);
        var manager = new GameManager(router, sync, stats, eviction, expiration, store);
        return (new GameService(manager), manager, sync, store);
    }
}
