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
    public static IEnumerable<string> ByName(this GroupNames kind) => kind.ToString().Split(", ");
}