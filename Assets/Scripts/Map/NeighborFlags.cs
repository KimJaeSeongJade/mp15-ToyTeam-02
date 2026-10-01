using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
enum NeighborFlags
{
    None = 0,

    Right = 1 << 0,
    Left = 1 << 1,
    Forward = 1 << 2,
    Back = 1 << 3
}
