using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int BlockType { get; }
    public float Weight { get; set; }
    private Dictionary<Direction, HashSet<int>> _rules = new();
    public Dictionary<Direction, HashSet<int>> Rules => _rules;

    public Tile(int blockType, float weight)
    {
        BlockType = blockType;
        Weight = weight;

        _rules[Direction.Up] = new HashSet<int>();
        _rules[Direction.Right] = new HashSet<int>();
        _rules[Direction.Down] = new HashSet<int>();
        _rules[Direction.Left] = new HashSet<int>();
    }

    public void AddNeighbor(Direction direction, int blockType)
    {
        _rules[direction].Add(blockType);
    }
}
