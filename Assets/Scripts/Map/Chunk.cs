using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private SplineManager _splineManager;
    private MeshBuilder _meshBuilder;
    private int _chunkIndex;
    private int[,] _chunkMap;
    private HashSet<IPoolable> _loadedPoolables = new();
    private IInteractable[,] _holdableMap = new IInteractable[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];
    private Dictionary<BlockType, BlockTypeColor> _blockTypeColors;

    public int ChunkIndex => _chunkIndex;
    public int[,] ChunkMap => _chunkMap;
    public IInteractable[,] HoldableMap => _holdableMap;

    private void Awake() => CacheComponents();

    /// <summary>
    /// 생성할 Chunk의 정보를 저장한다.
    /// </summary>
    /// <param name="chunkIndex"> Chunk의 번호 </param>
    /// <param name="map"></param>
    /// <param name="material"></param>
    /// <param name="textureSize"></param>
    /// <param name="atlasSize"></param>
    public void SetChunkData(int chunkIndex, SplineManager splineManager, int[,] chunkMap,
        Dictionary<BlockType, BlockTypeColor> blockTypeColors, UnityEngine.Material material, float textureSize, float atlasSize)
    {
        _chunkIndex = chunkIndex;
        _splineManager = splineManager;
        _chunkMap = chunkMap;
        _blockTypeColors = blockTypeColors;

        _loadedPoolables.Clear();
        ResetHoldableMap();

        _meshBuilder.CreateChunkMesh(chunkIndex, chunkMap, blockTypeColors, material, textureSize, atlasSize);
        SpawnPoolObjects();
    }

    private void SpawnPoolObjects()
    {
        for (int y = 0; y < _chunkMap.GetLength(1); y++)
        {
            for (int x = 0; x < _chunkMap.GetLength(0); x++)
            {
                BlockType blocktype = (BlockType)_chunkMap[x, y];
                Vector2Int coord = new Vector2Int(x + _chunkIndex * ChunkManager.CHUNK_SIZE, y);

                if (blocktype == BlockType.Rail)
                {
                    if (_chunkIndex == 0)
                    {
                        RailManager.Instance.TryRailwayPlace(coord);
                        continue;
                    }
                    else
                    {
                        RailManager.Instance.PlaceEndRailway(coord);
                        continue;
                    }
                }

                IPoolable newPoolable = ObjectPool.Instance.Take(blocktype);

                if (newPoolable == null) continue;

                newPoolable.GameObject.transform.position = coord.CoordToWorld();
                newPoolable.GameObject.SetActive(true);


                if ((int)blocktype >= Map.HOLDABLE_BLOCKTYPE_OFFSET)
                {
                    IInteractable interactable = newPoolable as IInteractable;

                    if (interactable != null)
                    {
                        Map.Instance.SetHoldable(coord, newPoolable as IInteractable);
                    }
                }

                AddLoadedPoolable(newPoolable);
            }
        }
    }

    /// <summary>
    /// 청크맵 좌표의 블록 종류를 반환
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <returns></returns>
    public BlockType GetBlockType(Vector2Int chunkCoord)
    {
        return (BlockType)_chunkMap[chunkCoord.x, chunkCoord.y];
    }


    /// <summary>
    /// 청크맵 좌표의 플레이어가 들 수 있는 오브젝트를 반환
    /// </summary>
    /// <param name="coord"> 청크맵 좌표 </param>
    /// <returns></returns>
    public IInteractable GetHoldable(Vector2Int chunkCoord)
    {
        return _holdableMap[chunkCoord.x, chunkCoord.y];
    }

    /// <summary>
    /// 플레이어가 들 수 있는 오브젝트를 청크맵 좌표에 저장
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <param name="interactable"> 플레이어가 들 수 있는 오브젝트 </param>
    public void SetHoldable(Vector2Int chunkCoord, IInteractable interactable)
    {
        _holdableMap[chunkCoord.x, chunkCoord.y] = interactable;
    }

    /// <summary>
    /// 로드한 Poolable hashset에서 poolable 추가
    /// </summary>
    /// <param name="poolable"> IPoolable 오브젝트 </param>
    public void AddLoadedPoolable(IPoolable poolable)
    {
        _loadedPoolables.Add(poolable);
    }

    /// <summary>
    /// 로드한 Poolable hashset에서 poolable 제거
    /// </summary>
    /// <param name="poolable"> IPoolable 오브젝트 </param>
    public void RemoveLoadedPoolable(IPoolable poolable)
    {
        _loadedPoolables.Remove(poolable);
    }

    /// <summary>
    /// 청크 위 Poolable 오브젝트를 오브젝트 풀로 반환
    /// </summary>
    public void DespawnPoolables()
    {
        foreach (IPoolable poolable in _loadedPoolables)
        {
            poolable.ReturnToPool();
            poolable.GameObject.SetActive(false);
        }
    }

    private void ResetHoldableMap()
    {
        for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
            {
                _holdableMap[x, y] = null;
            }
        }
    }

    private void CacheComponents()
    {
        _meshBuilder = GetComponent<MeshBuilder>();
    }
}
