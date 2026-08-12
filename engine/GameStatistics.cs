namespace GameEngine.Engine;

public sealed class GameStatistics
{
    private long _creates;
    private long _joins;
    private long _movesAccepted;
    private long _movesRejected;
    private long _leaves;
    private int _activeSessions;
    private int _activePlayers;
    private int _nodeSlotCount;

    public long CreateCount => Interlocked.Read(ref _creates);
    public long JoinCount => Interlocked.Read(ref _joins);
    public long MovesAccepted => Interlocked.Read(ref _movesAccepted);
    public long MovesRejected => Interlocked.Read(ref _movesRejected);
    public long LeaveCount => Interlocked.Read(ref _leaves);
    public int ActiveSessions => Volatile.Read(ref _activeSessions);
    public int ActivePlayers => Volatile.Read(ref _activePlayers);
    public int NodeSlotCount => Volatile.Read(ref _nodeSlotCount);

    public void RecordCreate() => Interlocked.Increment(ref _creates);
    public void RecordJoin() => Interlocked.Increment(ref _joins);
    public void RecordMoveAccepted() => Interlocked.Increment(ref _movesAccepted);
    public void RecordMoveRejected() => Interlocked.Increment(ref _movesRejected);
    public void RecordLeave() => Interlocked.Increment(ref _leaves);

    public void SetActiveSessions(int value) => Volatile.Write(ref _activeSessions, value);
    public void SetActivePlayers(int value) => Volatile.Write(ref _activePlayers, value);
    public void SetNodeSlotCount(int value) => Volatile.Write(ref _nodeSlotCount, value);

    public object Snapshot() => new
    {
        create_count = CreateCount,
        join_count = JoinCount,
        moves_accepted = MovesAccepted,
        moves_rejected = MovesRejected,
        leave_count = LeaveCount,
        active_sessions = ActiveSessions,
        active_players = ActivePlayers,
        node_session_slot_count = NodeSlotCount
    };
}
