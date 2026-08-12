using GameEngine.Shared;
using Xunit;

namespace GameEngine.Shared.Tests;

public class GameEntryTest
{
    [Fact]
    public void Clone_CopiesIndependentCollections()
    {
        var entry = new GameEntry
        {
            GameId = "game:1001",
            Owner = "Visvantha",
            Players = { "Visvantha" },
            Board = { ["cell-0"] = "X" },
            Turn = 1
        };

        var clone = entry.Clone();
        clone.Players.Add("Player2");
        clone.Board["cell-1"] = "O";

        Assert.Single(entry.Players);
        Assert.False(entry.Board.ContainsKey("cell-1"));
        Assert.Equal(2, clone.Players.Count);
    }
}

public class VersionInfoTest
{
    [Fact]
    public void FromEnvironment_SetsFields()
    {
        var v = VersionInfo.FromEnvironment("6", "8", "CSharp_FE6_BE8");
        Assert.Equal("6", v.FrontendDotnet);
        Assert.Equal("8", v.BackendDotnet);
        Assert.Equal("CSharp_FE6_BE8", v.Branch);
        Assert.Equal("ready", v.GameEngine);
    }

    [Fact]
    public void ParseBranch_RejectsSameVersion()
    {
        Assert.Throws<InvalidOperationException>(() => BuildContext.ParseBranch("CSharp_FE8_BE8"));
    }

    [Fact]
    public void ParseBranch_MapsFeBeCorrectly()
    {
        var ctx = BuildContext.ParseBranch("CSharp_FE6_BE8");
        Assert.Equal(6, ctx.FrontendVersion);
        Assert.Equal(8, ctx.BackendVersion);
        Assert.Equal("net6.0", ctx.FrontendTfm);
        Assert.Equal("net8.0", ctx.BackendTfm);
        Assert.Equal(".NET 6 / .NET 8", ctx.CustomerVersion);
    }
}
