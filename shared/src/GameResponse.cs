namespace GameEngine.Shared;

public sealed class GameResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = "OK";
    public string Message { get; set; } = string.Empty;
    public GameEntry? Game { get; set; }
    public object? Stats { get; set; }
}
