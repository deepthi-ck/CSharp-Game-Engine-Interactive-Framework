namespace GameEngine.Shared;

public sealed class VersionInfo
{
    public string Application { get; set; } = "C# Game Engine / Interactive Framework";
    public string FrontendDotnet { get; set; } = string.Empty;
    public string BackendDotnet { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string GameEngine { get; set; } = "ready";

    public static VersionInfo FromEnvironment(
        string frontend,
        string backend,
        string branch,
        string gameEngine = "ready") => new()
    {
        FrontendDotnet = frontend,
        BackendDotnet = backend,
        Branch = branch,
        GameEngine = gameEngine
    };
}
