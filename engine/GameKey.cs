namespace GameEngine.Engine;

public readonly record struct GameKey(string Value)
{
    public override string ToString() => Value;
    public static implicit operator string(GameKey key) => key.Value;
    public static GameKey Of(string value) => new(value);
}
