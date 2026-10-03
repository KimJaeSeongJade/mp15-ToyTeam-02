using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
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
    public void SetChunkData(int chunkIndex, int[,] worldMap, Material material, float textureSize, float atlasSize)
    {
        _chunkIndex = chunkIndex;
        _worldMap = worldMap;

        _meshBuilder.CreateChunkMesh(chunkIndex, worldMap, material, textureSize, atlasSize);
        SpawnPoolObjects();
    }

    private void SpawnPoolObjects()
    {
        for (int y = 0; y < _worldMap.GetLength(1); y++)
        {
            for (int x = _chunkIndex * ChunkManager.CHUNK_SIZE; x < (_chunkIndex + 1) * ChunkManager.CHUNK_SIZE; x++)
            {
                if (x >= _worldMap.GetLength(0)) return;

                IPoolable newPoolable = ObjectPool.Instance.Take((BlockType)_worldMap[x, y]);

                if (newPoolable == null) continue;

                Vector2Int coord = new Vector2Int(x, y);
                newPoolable.GameObject.transform.position = coord.CoordToWorld();
                newPoolable.GameObject.SetActive(true);
            }
        }
    }

    private void SpawnInvisibleColliders()
    {

    }

    private void CacheComponents()
    {
        _meshBuilder = GetComponent<MeshBuilder>();
    }
}
