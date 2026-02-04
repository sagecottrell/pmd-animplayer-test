using System;
using System.Collections.Generic;

namespace breakout;

[Flags]
public enum GroupNames
{
    Units = 0x01,
    Buildings = 0x02,
}
public static class GroupNamesExtensions
{
    public static IEnumerable<string> ByName(this GroupNames kind)
    {
        foreach (var val in Enum.GetValues<GroupNames>())
        {
            if (kind.HasFlag(val))
                yield return val.ToString();
        }
    }
}