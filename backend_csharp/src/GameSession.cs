using GameEngine.Distribution;
using GameEngine.Shared;

namespace GameEngine.Backend;

/// <summary>Backend facade over a routed primary GameSessionNode.</summary>
public sealed class GameSession
{
    private readonly GameSessionNode _node;

    public GameSession(GameSessionNode node) => _node = node;

    public int Slot => _node.Slot;
    public GameEntry Create(string gameId, string owner) => _node.Create(gameId, owner);
    public GameEntry Join(string gameId, string player) => _node.Join(gameId, player);
    public GameEntry Play(string gameId, string player, string move) => _node.Play(gameId, player, move);
    public GameEntry? GetState(string gameId) => _node.GetState(gameId);
    public bool Leave(string gameId, string? player = null) => _node.Leave(gameId, player);
}
