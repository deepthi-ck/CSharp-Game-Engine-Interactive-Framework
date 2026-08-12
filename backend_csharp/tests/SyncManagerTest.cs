using GameEngine.Shared;
using Xunit;

namespace GameEngine.Backend.Tests;

public class SyncManagerTest
{
    [Fact]
    public async Task Publish_NotifiesSubscribers()
    {
        var (_, manager, sync, _) = TestHost.Create();
        GameEntry? observed = null;
        using var sub = sync.Subscribe("game:sync", e => observed = e);

        await manager.CreateAsync("game:sync", "Visvantha");
        await manager.MoveAsync("game:sync", "Visvantha", "play:1");

        Assert.NotNull(observed);
        Assert.Equal("play:1", observed!.Board["last_move"]);
    }
}
