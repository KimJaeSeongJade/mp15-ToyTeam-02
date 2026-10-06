using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    /// <summary>
    /// 청크 한 변의 길이
    /// </summary>
    public const int CHUNK_SIZE = 20;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _targetTransform; // 청크 스트리밍 기준 타켓의 transform
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private GameObject _invisibleWallPrefab;
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private UnityEngine.Material _material;
    [SerializeField] private float _textureSize = 32f;
    [SerializeField] private int _atlasSize = 4;
    [SerializeField] private int _renderDistance = 2;

    private Queue<GameObject> _invisibleWallPool = new();
    private Queue<GameObject> _loadedInvisibleWalls = new();
    private Dictionary<int, Chunk> _chunks = new();
    private Queue<Chunk> _chunkPool = new();
    private Queue<Chunk> _loadedChunks = new();
    private int[,] _worldMap => _mapLoader.WorldMap;
    private int _previousChunkIndex;
    private int _oldestChunkIndex;
    private bool _canLoadMap => _mapLoader.CanLoadMap;

    public Dictionary<int, Chunk> Chunks => _chunks;

    private void Awake() => Init();
    private void Update() => UpdateStreaming();

    private void UpdateStreaming()
    {
        if (!_canLoadMap || _targetTransform == null) return;

        if (_loadedChunks.Count == 0) InitialLoad();

        int currentChunkIndex = GetChunkIndex(_targetTransform.position);

        if (currentChunkIndex <= _previousChunkIndex) return;

        if (currentChunkIndex - _renderDistance > _oldestChunkIndex)
        {
            RemoveOldestChunk();
            RemoveOldestInvisibleWalls();
        }

        _previousChunkIndex = currentChunkIndex;

        LoadChunk(currentChunkIndex + _renderDistance);
        LoadInvisibleWall(currentChunkIndex + _renderDistance);
    }

    private int GetChunkIndex(Vector3 position)
    {
        return (int)position.x / CHUNK_SIZE;
    }

    private void LoadChunk(int chunkIndex)
    {
        if (_chunkPool.Count == 0) return;

        Chunk chunk = _chunkPool.Dequeue();

        int[,] chunkMap = WorldMapToChunkMap(chunkIndex);

        if (!_chunks.ContainsKey(chunkIndex))
            _chunks.Add(chunkIndex, chunk);

        if (chunkMap == null) return;

        chunk.SetChunkData(chunkIndex, _splineManager, WorldMapToChunkMap(chunkIndex), _material, _textureSize, _atlasSize);
        chunk.gameObject.SetActive(true);

        _loadedChunks.Enqueue(chunk);
    }

    private void LoadInvisibleWall(int chunkIndex)
    {
        if (_invisibleWallPool.Count == 0) return;

        GameObject invisibleWallTop = _invisibleWallPool.Dequeue();
        GameObject invisibleWallBottom = _invisibleWallPool.Dequeue();

        invisibleWallTop.transform.position = new Vector3(chunkIndex * CHUNK_SIZE, 0f, -1f);
        invisibleWallBottom.transform.position = new Vector3(chunkIndex * CHUNK_SIZE, 0f, 20f);


        invisibleWallTop.SetActive(true);
        invisibleWallBottom.SetActive(true);

        _loadedInvisibleWalls.Enqueue(invisibleWallTop);
        _loadedInvisibleWalls.Enqueue(invisibleWallBottom);
    }

    private void RemoveOldestChunk()
    {
        if (_loadedChunks.Count == 0) return;

        Chunk chunk = _loadedChunks.Dequeue();
        _chunks.Remove(chunk.ChunkIndex);

        chunk.DespawnPoolables();

        chunk.gameObject.SetActive(false);
        _chunkPool.Enqueue(chunk);
    }

    private void RemoveOldestInvisibleWalls()
    {
        if (_loadedInvisibleWalls.Count == 0) return;

        GameObject invisibleWallTop = _loadedInvisibleWalls.Dequeue();
        GameObject invisibleWallBottom = _loadedInvisibleWalls.Dequeue();

        invisibleWallTop.SetActive(false);
        invisibleWallBottom.SetActive(false);

        _invisibleWallPool.Enqueue(invisibleWallTop);
        _invisibleWallPool.Enqueue(invisibleWallBottom);
    }

    private void InitialLoad()
    {
        for (int i = 0; i <= _renderDistance; i++)
        {
            LoadChunk(i);
            LoadInvisibleWall(i);
        }

        _splineManager.LoadTrain();
        _oldestChunkIndex = 0;
        _playerController.gameObject.SetActive(true);
        Debug.Log("Map Loaded!");
    }

    private void CreateChunkPool()
    {
        for (int i = 0; i < _renderDistance * 2 + 1; i++)
        {
            Chunk newChunk = Instantiate(_chunkPrefab, transform);

            newChunk.gameObject.SetActive(false);

            _chunkPool.Enqueue(newChunk);
        }
    }

    private void CreateInvisibleWallPool()
    {
        for (int i = 0; i < _chunkPool.Count * 2; i++)
        {
            GameObject invisibleWall = Instantiate(_invisibleWallPrefab, transform);

            invisibleWall.gameObject.SetActive(false);

            _invisibleWallPool.Enqueue(invisibleWall);
        }
    }

    private int[,] WorldMapToChunkMap(int chunkIndex)
    {
        int[,] chunkMap = new int[CHUNK_SIZE, CHUNK_SIZE];

        if (_worldMap.GetLength(0) < chunkIndex * CHUNK_SIZE) return null;

        for (int y = 0; y < _worldMap.GetLength(1); y++)
        {
            for (int x = chunkIndex * CHUNK_SIZE; x < (chunkIndex + 1) * CHUNK_SIZE; x++)
            {
                if (x >= _worldMap.GetLength(0))
                {
                    chunkMap[x - chunkIndex * CHUNK_SIZE, y] = 0;
                }
                else
                {
                    chunkMap[x - chunkIndex * CHUNK_SIZE, y] = _worldMap[x, y];
                }
            }
        }
        return chunkMap;
    }

    public Chunk GetChunk(Vector2Int coord)
    {
        return _chunks[coord.x / CHUNK_SIZE];
    }

    private void Init()
    {
        CreateChunkPool();
        CreateInvisibleWallPool();
    }
}
