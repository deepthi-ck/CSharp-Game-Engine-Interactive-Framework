namespace GameEngine.Shared;

public sealed class GameRequest
{
    public string? GameId { get; set; }
    public string? Player { get; set; }
    public string? Action { get; set; }
    public string? Move { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}
