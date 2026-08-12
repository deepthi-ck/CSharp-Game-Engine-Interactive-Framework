using GameEngine.Shared;
using Xunit;

namespace GameEngine.Backend.Tests;

public class GameServiceTest
{
    [Fact]
    public async Task CreateJoinMoveLeave_Flow()
    {
        var (svc, _, _, _) = TestHost.Create();
        var created = await svc.CreateAsync(new GameRequest { GameId = "game:1001", Player = "Visvantha" });
        Assert.True(created.Success);
        Assert.Contains("SUCCESS", created.Message);

        var joined = await svc.JoinAsync("game:1001", new GameRequest { Player = "Player2" });
        Assert.True(joined.Success);
        Assert.Equal(2, joined.Game!.Players.Count);

        var moved = await svc.MoveAsync("game:1001", new GameRequest { Player = "Visvantha", Move = "play:card-A" });
        Assert.True(moved.Success);
        Assert.Equal("play:card-A", moved.Game!.Board["last_move"]);

        var left = await svc.LeaveAsync("game:1001");
        Assert.True(left.Success);
        Assert.Equal("ENDED", left.Status);

        var get = svc.Get("game:1001");
        Assert.False(get.Success);
        Assert.Equal("ENDED", get.Status);
    }
}
