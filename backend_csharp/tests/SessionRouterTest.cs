using GameEngine.Distribution;
using GameEngine.Engine;
using Xunit;

namespace GameEngine.Backend.Tests;

public class SessionRouterTest
{
    [Fact]
    public void SameGameId_RoutesConsistently()
    {
        var policy = new GamePolicy { SessionSlotCount = 3 };
        var store = new InMemoryGameStore();
        var registry = new SessionRegistry();
        for (var i = 0; i < 3; i++)
            registry.Register(new GameSessionNode(i, store, policy));
        var router = new SessionRouter(registry, 3);

        var a = router.ResolveSlot("game:1001");
        var b = router.ResolveSlot("game:1001");
        Assert.Equal(a, b);
        Assert.InRange(a, 0, 2);
        Assert.Equal(router.Route("game:1001").Slot, a);
    }
}
