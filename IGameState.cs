using breakout.customResources;
using System.Collections.Generic;

namespace breakout;

public interface IGameState
{
    bool GetResourceCount(GameResourceNames r, out long value);
    long GetResourceCount(GameResourceNames r)
    {
        GetResourceCount(r, out long value);
        return value;
    }

    bool TryBuy(IDictionary<GameResourceNames, long> values);

    TeamInfo? PlayerTeam { get; }
}
