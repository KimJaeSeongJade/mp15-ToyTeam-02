using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPath : MonoBehaviour
{
    public bool Collapsed;
    public TilePath[] TileOptions;

    public void CreateCell(bool collapsed, TilePath[] tiles)
    {
        this.Collapsed = collapsed;
        TileOptions = tiles;
    }

    public void RecreateCell(TilePath[] tiles)
    {
        TileOptions = tiles;
    }
}
