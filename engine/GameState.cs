using GameEngine.Shared;

namespace GameEngine.Engine;

/// <summary>Authoritative in-memory game state wrapper around shared GameEntry.</summary>
public sealed class GameState
{
    public GameEntry Entry { get; }
    public DateTimeOffset LastActivityUtc { get; set; } = DateTimeOffset.UtcNow;

    public GameState(GameEntry entry) => Entry = entry;

    public void Touch() => LastActivityUtc = DateTimeOffset.UtcNow;
}
