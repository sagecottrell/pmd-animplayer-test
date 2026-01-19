using breakout.resourceTypes;

namespace breakout;

public interface IGameState
{
    bool GetResourceCount(ResourceNames r, out long value);
    long GetResourceCount(ResourceNames r)
    {
        GetResourceCount(r, out long value);
        return value;
    }

    bool TryBuy(Godot.Collections.Dictionary<ResourceNames, long> values);

    TeamInfo? PlayerTeam { get; }
}
