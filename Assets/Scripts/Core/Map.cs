using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 들 수 있는 오브젝트의 블록 종류 오프셋
    /// </summary>
    public const int HOLDABLE_BLOCKTYPE_OFFSET = 7;

    /// <summary>
    /// 맵 싱글톤
    /// </summary>
    public static Map Instance;

    private int[,] _worldMap;
    private IInteractable[,] _holdableMap;

    private void Awake()
    {
        SetSingleton();
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 맵 좌표의 블록 종류를 반환
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <returns></returns>
    public BlockType GetBlockType(Vector2Int coord)
    {
        return (BlockType)_worldMap[coord.x, coord.y];
    }

    /// <summary>
    /// 맵 좌표의 플레이어가 들 수 있는 오브젝트를 반환
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <returns></returns>
    public IInteractable GetHoldable(Vector2Int coord)
    {
        if (_holdableMap == null) return null;
        return _holdableMap[coord.x, coord.y];
    }

    /// <summary>
    /// 플레이어가 들 수 있는 오브젝트를 맵 좌표에 저장
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <param name="interactable"> 플레이어가 들 수 있는 오브젝트 </param>
    public void SetHoldable(Vector2Int coord, IInteractable interactable)
    {
        Debug.Log($"SetHoldable {coord} {interactable?.GameObject.name}");
        _holdableMap[coord.x, coord.y] = interactable;
    }

    /// <summary>
    /// 맵 데이터 저장
    /// </summary>
    /// <param name="mapData"> 맵 데이터 </param>
    public void SetMapData(int[,] mapData)
    {
        _worldMap = mapData;
        _holdableMap = new IInteractable[mapData.GetLength(0), mapData.GetLength(1)];
    }
}