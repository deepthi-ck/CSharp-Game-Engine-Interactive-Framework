using System.Security.Cryptography;
using System.Text;

namespace GameEngine.Distribution;

/// <summary>Consistent GameId → partition/slot → primary session mapping.</summary>
public sealed class SessionRouter
{
    private readonly SessionRegistry _registry;
    private readonly int _slotCount;

    public SessionRouter(SessionRegistry registry, int slotCount)
    {
        _registry = registry;
        _slotCount = Math.Max(1, slotCount);
    }

    public int ResolveSlot(string gameId)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(gameId));
        var value = BitConverter.ToUInt32(hash, 0);
        return (int)(value % (uint)_slotCount);
    }

    public GameSessionNode Route(string gameId)
    {
        var slot = ResolveSlot(gameId);
        return _registry.Get(slot);
    }
}
