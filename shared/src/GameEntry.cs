namespace GameEngine.Shared;

public sealed class GameEntry
{
    public string GameId { get; set; } = string.Empty;
    public List<string> Players { get; set; } = new();
    public Dictionary<string, string> Board { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; set; }
    public int Revision { get; set; }
    public int Turn { get; set; }
    public string Status { get; set; } = "active";
    public string Owner { get; set; } = string.Empty;
    public string SessionSlot { get; set; } = string.Empty;

    public GameEntry Clone() => new()
    {
        GameId = GameId,
        Players = new List<string>(Players),
        Board = new Dictionary<string, string>(Board),
        CreatedAt = CreatedAt,
        ExpiresAt = ExpiresAt,
        Revision = Revision,
        Turn = Turn,
        Status = Status,
        Owner = Owner,
        SessionSlot = SessionSlot
    };
}
