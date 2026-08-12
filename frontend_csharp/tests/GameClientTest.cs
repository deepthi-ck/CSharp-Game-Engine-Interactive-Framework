using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GameEngine.Frontend;
using GameEngine.Shared;
using RichardSzalay.MockHttp;
using Xunit;

namespace GameEngine.Frontend.Tests;

public class GameClientTest
{
    [Fact]
    public async Task CreateAsync_PostsToGameApi()
    {
        var mock = new MockHttpMessageHandler();
        mock.When(HttpMethod.Post, "http://localhost/game")
            .Respond("application/json", JsonSerializer.Serialize(new GameResponse
            {
                Success = true,
                Status = "OK",
                Message = "CREATE = SUCCESS",
                Game = new GameEntry { GameId = "game:1001", Owner = "Visvantha", Players = { "Visvantha" } }
            }));

        var http = mock.ToHttpClient();
        http.BaseAddress = new Uri("http://localhost/");
        var client = new GameClient(http);
        var result = await client.CreateAsync("game:1001", "Visvantha");

        Assert.NotNull(result);
        Assert.True(result!.Success);
        Assert.Equal("game:1001", result.Game!.GameId);
    }
}
