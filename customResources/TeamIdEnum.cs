
using System;

namespace breakout.customResources;

[Flags]
public enum TeamIdEnum
{
    Player1 = 0x1,

    Enemy1 = 0x100,
    Enemy2 = 0x200,
    Enemy3 = 0x400,
    Enemy4 = 0x800,
    //Enemy5 = 0x1000,
    //Enemy6 = 0x2000,
    //Enemy7 = 0x4000,
    //Enemy8 = 0x8000,
}
