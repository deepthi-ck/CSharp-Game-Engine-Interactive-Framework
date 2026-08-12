namespace GameEngine.Engine;

public sealed class GamePolicy
{
    public int SessionTtlSeconds { get; init; } = 300;
    public int MaxSessions { get; init; } = 32;
    public int MaxPlayersPerSession { get; init; } = 8;
    public int SessionSlotCount { get; init; } = 3;

    public static GamePolicy FromConfiguration(GameEngine.Shared.GameConfiguration cfg) => new()
    {
        SessionTtlSeconds = cfg.SessionTtlSeconds,
        MaxSessions = cfg.MaxSessions,
        MaxPlayersPerSession = cfg.MaxPlayersPerSession,
        SessionSlotCount = cfg.SessionSlotCount
    };
}
