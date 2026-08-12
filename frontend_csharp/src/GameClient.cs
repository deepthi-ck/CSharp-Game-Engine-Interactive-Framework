using System.Net.Http.Json;
using GameEngine.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace GameEngine.Frontend;

public sealed class GameClient : IAsyncDisposable
{
    private readonly HttpClient _http;
    private HubConnection? _hub;

    public GameClient(HttpClient http) => _http = http;

    public string? LastError { get; private set; }
    public GameEntry? LastGame { get; private set; }
    public event Action? StateChanged;

    public async Task<GameResponse?> CreateAsync(string gameId, string player)
    {
        var response = await _http.PostAsJsonAsync("/game", new GameRequest { GameId = gameId, Player = player });
        return await ReadAsync(response);
    }

    public async Task<GameResponse?> JoinAsync(string gameId, string player)
    {
        var response = await _http.PostAsJsonAsync($"/game/{Uri.EscapeDataString(gameId)}/join", new GameRequest { Player = player });
        return await ReadAsync(response);
    }

    public async Task<GameResponse?> MoveAsync(string gameId, string player, string move)
    {
        var response = await _http.PostAsJsonAsync($"/game/{Uri.EscapeDataString(gameId)}/move",
            new GameRequest { Player = player, Move = move });
        return await ReadAsync(response);
    }

    public async Task<GameResponse?> GetAsync(string gameId)
    {
        var response = await _http.GetAsync($"/game/{Uri.EscapeDataString(gameId)}");
        return await ReadAsync(response);
    }

    public async Task<GameResponse?> LeaveAsync(string gameId)
    {
        var response = await _http.DeleteAsync($"/game/{Uri.EscapeDataString(gameId)}");
        return await ReadAsync(response);
    }

    public Task<object?> StatsAsync() => _http.GetFromJsonAsync<object>("/game/stats");
    public Task<object?> HealthAsync() => _http.GetFromJsonAsync<object>("/health");
    public Task<object?> VersionAsync() => _http.GetFromJsonAsync<object>("/version");

    public async Task ConnectHubAsync(string hubUrl, string gameId, Action<GameEntry> onUpdate)
    {
        _hub = new HubConnectionBuilder().WithUrl(hubUrl).WithAutomaticReconnect().Build();
        _hub.On<GameEntry>("GameUpdated", entry =>
        {
            LastGame = entry;
            onUpdate(entry);
            StateChanged?.Invoke();
        });
        _hub.On<GameEntry>("MoveAccepted", entry =>
        {
            LastGame = entry;
            onUpdate(entry);
            StateChanged?.Invoke();
        });
        await _hub.StartAsync();
        await _hub.InvokeAsync("JoinRoom", gameId, "observer");
    }

    private async Task<GameResponse?> ReadAsync(HttpResponseMessage response)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<GameResponse>();
            if (payload is null)
            {
                LastError = $"Empty response ({(int)response.StatusCode})";
                return null;
            }
            LastError = payload.Success ? null : payload.Message;
            LastGame = payload.Game ?? LastGame;
            StateChanged?.Invoke();
            return payload;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            StateChanged?.Invoke();
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null)
            await _hub.DisposeAsync();
    }
}
