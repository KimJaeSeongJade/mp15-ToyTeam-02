using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool Collapsed;
    public Tile[] TileOptions;

    /// <summary>
    /// 한 Cell의 후보 설정
    /// </summary>
    /// <param name="collapsed"> 붕괴 여부 </param>
    /// <param name="tiles"> 후보 </param>
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
