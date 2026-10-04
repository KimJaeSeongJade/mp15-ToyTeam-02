using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool Collapsed;
    public Tile[] TileOptions;

    public void CreateCell(bool collapsed, Tile[] tiles)
    {
        this.Collapsed = collapsed;
        TileOptions = tiles;
    }

    public void RecreateCell(Tile[] tiles)
    {
        TileOptions = tiles;
    }
}
