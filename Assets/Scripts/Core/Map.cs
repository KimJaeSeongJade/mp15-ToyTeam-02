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
    public const int HOLDABLE_BLOCKTYPE_OFFSET = (int)BlockType.Rail;

    /// <summary>
    /// 맵 싱글톤
    /// </summary>
    public static Map Instance;

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

        if (!_chunkManager.Chunks.ContainsKey(chunkIndex)) return null;

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
    /// Chunk의 로딩된 Poolable로 등록 (현재 Chunk에서 게임 화면이 멀어지면 풀로 되돌아감)
    /// </summary>
    /// <param name="poolable"></param>
    public void AddLoadedPoolable(IPoolable poolable)
    {
        Chunk chunk = _chunkManager.GetChunk(poolable.GameObject.transform.position.WorldToCoord());
        chunk.AddLoadedPoolable(poolable);
    }

    /// <summary>
    /// Chunk의 로딩된 Poolable로 등록 해제 (현재 Chunk에서 게임 화면이 멀어져도 풀로 되돌아가지 않음)
    /// </summary>
    /// <param name="poolable"></param>
    public void RemoveLoadedPoolable(IPoolable poolable)
    {
        if (poolable == null) return;
        Chunk chunk = _chunkManager.GetChunk(poolable.GameObject.transform.position.WorldToCoord());
        chunk.RemoveLoadedPoolable(poolable);
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