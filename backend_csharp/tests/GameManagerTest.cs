using Xunit;

namespace GameEngine.Backend.Tests;

public class GameManagerTest
{
    [Fact]
    public async Task Statistics_ReflectOperations()
    {
        var (_, manager, _, _) = TestHost.Create();
        await manager.CreateAsync("game:stats", "Visvantha");
        await manager.JoinAsync("game:stats", "guest01");
        await manager.MoveAsync("game:stats", "Visvantha", "move-1");
        await manager.MoveAsync("game:stats", "nobody", "bad");
        await manager.LeaveAsync("game:stats");

        Assert.Equal(1, manager.Stats.CreateCount);
        Assert.Equal(1, manager.Stats.JoinCount);
        Assert.Equal(1, manager.Stats.MovesAccepted);
        Assert.Equal(1, manager.Stats.MovesRejected);
        Assert.Equal(1, manager.Stats.LeaveCount);
    }

    [Fact]
    public async Task Ttl_ExpiresSession()
    {
        var (_, manager, _, _) = TestHost.Create(ttlSeconds: 1);
        await manager.CreateAsync("game:ttl", "Visvantha");
        await Task.Delay(1100);
        var get = manager.Get("game:ttl");
        Assert.False(get.Success);
        Assert.Equal("ENDED", get.Status);
    }

    [Fact]
    public async Task Eviction_RespectsCapacity()
    {
        var (_, manager, _, store) = TestHost.Create(maxSessions: 2);
        await manager.CreateAsync("game:e1", "A");
        await Task.Delay(20);
        await manager.CreateAsync("game:e2", "B");
        await Task.Delay(20);
        await manager.CreateAsync("game:e3", "C");
        Assert.True(store.Count <= 2);
        Assert.True(store.Contains("game:e3"));
    }
}
