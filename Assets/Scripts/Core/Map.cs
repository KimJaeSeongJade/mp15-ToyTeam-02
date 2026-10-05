using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private ChunkManager _chunkManager;

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
        int chunkIndex = GetChunkIndex(coord);
        Vector2Int chunkCoord = GetChunkCoord(coord);
        return _chunkManager.Chunks[chunkIndex].GetBlockType(chunkCoord);
    }

    /// <summary>
    /// 맵 좌표의 플레이어가 들 수 있는 오브젝트를 반환
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <returns></returns>
    public IInteractable GetHoldable(Vector2Int coord)
    {
        int chunkIndex = GetChunkIndex(coord);
        Vector2Int chunkCoord = GetChunkCoord(coord);
        return _chunkManager.Chunks[chunkIndex].GetHoldable(chunkCoord);
    }

    /// <summary>
    /// 플레이어가 들 수 있는 오브젝트를 맵 좌표에 저장
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <param name="interactable"> 플레이어가 들 수 있는 오브젝트 </param>
    public void SetHoldable(Vector2Int coord, IInteractable interactable)
    {
        int chunkIndex = GetChunkIndex(coord);
        Vector2Int chunkCoord = GetChunkCoord(coord);
        _chunkManager.Chunks[chunkIndex].SetHoldable(chunkCoord, interactable);
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

    private int GetChunkIndex(Vector2Int worldCoord)
    {
        return worldCoord.x / ChunkManager.CHUNK_SIZE;
    }

    private Vector2Int GetChunkCoord(Vector2Int worldCoord)
    {
        return new Vector2Int(worldCoord.x - ChunkManager.CHUNK_SIZE * GetChunkIndex(worldCoord), worldCoord.y);
    }
}