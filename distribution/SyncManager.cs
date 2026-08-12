using GameEngine.Shared;

namespace GameEngine.Distribution;

public interface IGameBroadcaster
{
    Task BroadcastGameUpdateAsync(string gameId, GameEntry entry, string eventName);
    Task BroadcastGameEndedAsync(string gameId);
}

/// <summary>Primary-session → connected-player sync (in-process + SignalR bridge).</summary>
public sealed class SyncManager
{
    private readonly Dictionary<string, List<Action<GameEntry>>> _observers = new(StringComparer.Ordinal);
    private readonly object _gate = new();
    private IGameBroadcaster? _broadcaster;

    public void SetBroadcaster(IGameBroadcaster broadcaster) => _broadcaster = broadcaster;

    public IDisposable Subscribe(string gameId, Action<GameEntry> observer)
    {
        lock (_gate)
        {
            if (!_observers.TryGetValue(gameId, out var list))
            {
                list = new List<Action<GameEntry>>();
                _observers[gameId] = list;
            }
            list.Add(observer);
        }
        return new Subscription(() => Unsubscribe(gameId, observer));
    }

    public void Unsubscribe(string gameId, Action<GameEntry> observer)
    {
        lock (_gate)
        {
            if (_observers.TryGetValue(gameId, out var list))
            {
                list.Remove(observer);
                if (list.Count == 0) _observers.Remove(gameId);
            }
        }
    }

    public async Task PublishAsync(string gameId, GameEntry entry, string eventName = "GameUpdated")
    {
        List<Action<GameEntry>> snapshot;
        lock (_gate)
        {
            snapshot = _observers.TryGetValue(gameId, out var list)
                ? list.ToList()
                : new List<Action<GameEntry>>();
        }

        foreach (var obs in snapshot)
            obs(entry);

        if (_broadcaster is not null)
            await _broadcaster.BroadcastGameUpdateAsync(gameId, entry, eventName);
    }

    public async Task PublishEndedAsync(string gameId)
    {
        lock (_gate) { _observers.Remove(gameId); }
        if (_broadcaster is not null)
            await _broadcaster.BroadcastGameEndedAsync(gameId);
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _dispose;
        public Subscription(Action dispose) => _dispose = dispose;
        public void Dispose() => _dispose();
    }
}
