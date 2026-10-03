using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private SplineManager _splineManager;
    private MeshBuilder _meshBuilder;
    private int _chunkIndex;
    private int[,] _worldMap;
    private void Awake() => CacheComponents();

    /// <summary>
    /// 생성할 Chunk의 정보를 저장한다.
    /// </summary>
    /// <param name="chunkIndex"> Chunk의 번호 </param>
    /// <param name="map"></param>
    /// <param name="material"></param>
    /// <param name="textureSize"></param>
    /// <param name="atlasSize"></param>
    public void SetChunkData(int chunkIndex, SplineManager splineManager, int[,] worldMap, Material material, float textureSize, float atlasSize)
    {
        _chunkIndex = chunkIndex;
        _splineManager = splineManager;
        _worldMap = worldMap;

        _meshBuilder.CreateChunkMesh(chunkIndex, worldMap, material, textureSize, atlasSize);
        SpawnPoolObjects();
        SpawnTrain();
    }

    private void SpawnPoolObjects()
    {
        for (int y = 0; y < _worldMap.GetLength(1); y++)
        {
            for (int x = _chunkIndex * ChunkManager.CHUNK_SIZE; x < (_chunkIndex + 1) * ChunkManager.CHUNK_SIZE; x++)
            {
                if (x >= _worldMap.GetLength(0)) continue;
                BlockType blocktype = (BlockType)_worldMap[x, y];
                IPoolable newPoolable = ObjectPool.Instance.Take(blocktype);

                if (newPoolable == null) continue;

                Vector2Int coord = new Vector2Int(x, y);
                newPoolable.GameObject.transform.position = coord.CoordToWorld();
                newPoolable.GameObject.SetActive(true);


                if ((int)blocktype >= Map.HOLDABLE_BLOCKTYPE_OFFSET)
                {
                    IInteractable interactable = newPoolable as IInteractable;
                    if (interactable != null)
                    {
                        Map.Instance.SetHoldable(coord, newPoolable as IInteractable);
                    }

                    if (blocktype == BlockType.Rail)
                    {
                        RailManager.Instance.AddRailWay(interactable as Rail);
                    }
                }
            }
        }
    }

    private void SpawnTrain()
    {
        if (_chunkIndex != 0) return;

        List<Vector2Int> initialRailway = new();

        for (int y = 0; y < _worldMap.GetLength(1); y++)
        {
            for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
            {
                if (x >= _worldMap.GetLength(0)) continue;

                if ((BlockType)_worldMap[x, y] == BlockType.Rail)
                {
                    initialRailway.Add(new Vector2Int(x, y));
                }
            }
        }
        // TODO: Instantiate train
        _splineManager.InitSpline(initialRailway);
    }

    private void CacheComponents()
    {
        _meshBuilder = GetComponent<MeshBuilder>();
    }
}
