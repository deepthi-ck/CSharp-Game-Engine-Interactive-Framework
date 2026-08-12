namespace GameEngine.Distribution;

public sealed class SessionRegistry
{
    private readonly Dictionary<int, GameSessionNode> _nodes = new();

    public void Register(GameSessionNode node) => _nodes[node.Slot] = node;

    public GameSessionNode Get(int slot) =>
        _nodes.TryGetValue(slot, out var node)
            ? node
            : throw new KeyNotFoundException($"Session slot not registered: {slot}");

    public IReadOnlyCollection<GameSessionNode> All => _nodes.Values;
    public int Count => _nodes.Count;
}
