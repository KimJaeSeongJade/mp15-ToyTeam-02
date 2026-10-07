using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockTypeColor
{
    public BlockType BlockType;
    public Dictionary<Vector2Int, float> BlockColors;

    public BlockTypeColor(BlockType blockType, Dictionary<Vector2Int, float> blockColors)
    {
        BlockType = blockType;
        BlockColors = blockColors;
    }
}
