using GameEngine.Shared;

namespace GameEngine.Engine;

public sealed class InMemoryGameStore
{
    private readonly Dictionary<string, GameState> _store = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public GameEntry CreateSession(string gameId, string owner, TimeSpan ttl, string sessionSlot)
    {
        lock (_gate)
        {
            if (_store.ContainsKey(gameId))
                throw new InvalidOperationException($"Session already exists: {gameId}");

            var entry = new GameEntry
            {
                GameId = gameId,
                Owner = owner,
                Players = { owner },
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.Add(ttl),
                SessionSlot = sessionSlot,
                Status = "active",
                Board = new Dictionary<string, string> { ["phase"] = "lobby" }
            };
            _store[gameId] = new GameState(entry);
            return entry.Clone();
        }
    }

    public GameEntry JoinSession(string gameId, string player, int maxPlayers)
    {
        lock (_gate)
        {
            var state = Require(gameId);
            EnsureActive(state);
            if (state.Entry.Players.Contains(player, StringComparer.OrdinalIgnoreCase))
                return state.Entry.Clone();
            if (state.Entry.Players.Count >= maxPlayers)
                throw new InvalidOperationException("Session player capacity reached.");
            state.Entry.Players.Add(player);
            state.Entry.Revision++;
            state.Touch();
            return state.Entry.Clone();
        }
    }

    public GameEntry PlayMove(string gameId, string player, string move)
    {
        lock (_gate)
        {
            var state = Require(gameId);
            EnsureActive(state);
            if (!state.Entry.Players.Contains(player, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Player not in session.");
            if (string.IsNullOrWhiteSpace(move))
                throw new InvalidOperationException("Move rejected: empty.");

            state.Entry.Board["last_move"] = move;
            state.Entry.Board["last_player"] = player;
            state.Entry.Board["phase"] = "playing";
            state.Entry.Turn++;
            state.Entry.Revision++;
            state.Touch();
            return state.Entry.Clone();
        }
    }

    public GameEntry? GetState(string gameId)
    {
        lock (_gate)
        {
            if (!_store.TryGetValue(gameId, out var state))
                return null;
            if (IsExpired(state))
            {
                _store.Remove(gameId);
                return null;
            }
            return state.Entry.Clone();
        }
    }

    public bool LeaveSession(string gameId, string? player = null)
    {
        lock (_gate)
        {
            if (!_store.TryGetValue(gameId, out var state))
                return false;

            if (player is null)
            {
                _store.Remove(gameId);
                return true;
            }

            state.Entry.Players.RemoveAll(p => string.Equals(p, player, StringComparison.OrdinalIgnoreCase));
            state.Entry.Revision++;
            state.Touch();
            if (state.Entry.Players.Count == 0)
            {
                _store.Remove(gameId);
            }
            return true;
        }
    }

    public bool Contains(string gameId)
    {
        lock (_gate)
        {
            if (!_store.TryGetValue(gameId, out var state))
                return false;
            if (IsExpired(state))
            {
                _store.Remove(gameId);
                return false;
            }
            return true;
        }
    }

    public void Clear()
    {
        lock (_gate) { _store.Clear(); }
    }

    public IReadOnlyList<string> AllIds()
    {
        lock (_gate) { return _store.Keys.ToList(); }
    }

    public int Count
    {
        get { lock (_gate) { return _store.Count; } }
    }

    public int PlayerCount
    {
        get
        {
            lock (_gate)
            {
                return _store.Values.Sum(s => s.Entry.Players.Count);
            }
        }
    }

    public List<string> EvictExpired()
    {
        lock (_gate)
        {
            var expired = _store.Where(kv => IsExpired(kv.Value)).Select(kv => kv.Key).ToList();
            foreach (var id in expired)
                _store.Remove(id);
            return expired;
        }
    }

    public string? EvictLeastRecentlyUsed()
    {
        lock (_gate)
        {
            if (_store.Count == 0) return null;
            var victim = _store.OrderBy(kv => kv.Value.LastActivityUtc).First();
            _store.Remove(victim.Key);
            return victim.Key;
        }
    }

    private GameState Require(string gameId)
    {
        if (!_store.TryGetValue(gameId, out var state))
            throw new KeyNotFoundException($"Game not found: {gameId}");
        return state;
    }

    private static void EnsureActive(GameState state)
    {
        if (IsExpired(state))
            throw new InvalidOperationException("Session expired.");
        if (!string.Equals(state.Entry.Status, "active", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Session not active.");
    }

    private static bool IsExpired(GameState state) =>
        state.Entry.ExpiresAt is { } exp && exp <= DateTimeOffset.UtcNow;
}
