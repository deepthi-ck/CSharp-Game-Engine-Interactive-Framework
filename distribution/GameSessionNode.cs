using GameEngine.Engine;
using GameEngine.Shared;

namespace GameEngine.Distribution;

public sealed class GameSessionNode
{
    private readonly InMemoryGameStore _store;
    private readonly GamePolicy _policy;

    public int Slot { get; }

    public GameSessionNode(int slot, InMemoryGameStore store, GamePolicy policy)
    {
        Slot = slot;
        _store = store;
        _policy = policy;
    }

    public GameEntry Create(string gameId, string owner) =>
        _store.CreateSession(gameId, owner, TimeSpan.FromSeconds(_policy.SessionTtlSeconds), $"slot-{Slot}");

    public GameEntry Join(string gameId, string player) =>
        _store.JoinSession(gameId, player, _policy.MaxPlayersPerSession);

    public GameEntry Play(string gameId, string player, string move) =>
        _store.PlayMove(gameId, player, move);

    public GameEntry? GetState(string gameId) => _store.GetState(gameId);

    public bool Leave(string gameId, string? player = null) => _store.LeaveSession(gameId, player);

    public bool Contains(string gameId) => _store.Contains(gameId);
}
