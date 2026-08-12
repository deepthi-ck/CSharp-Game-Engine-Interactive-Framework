namespace GameEngine.Shared;

public sealed class GameConfiguration
{
    public int SessionTtlSeconds { get; set; } = 300;
    public int MaxSessions { get; set; } = 32;
    public int MaxPlayersPerSession { get; set; } = 8;
    public int SessionSlotCount { get; set; } = 3;
    public string ApiBaseUrl { get; set; } = "http://localhost:5080";
    public string HubPath { get; set; } = "/hubs/game";
}
