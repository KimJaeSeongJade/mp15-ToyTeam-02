using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    /// <summary>
    /// 청크 한 변의 길이
    /// </summary>
    public const int CHUNK_SIZE = 20;
    public const int RENDER_DISTANCE = 2;

    [SerializeField] private WaveFunction _waveFunction;
    [SerializeField] private Transform _targetTransform; // 청크 스트리밍 기준 타켓의 transform
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private GameObject _invisibleWallPrefab;
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private UnityEngine.Material _material;
    [SerializeField] private float _textureSize = 32f;
    [SerializeField] private int _atlasSize = 4;

    private Queue<GameObject> _invisibleWallPool = new();
    private Queue<GameObject> _loadedInvisibleWalls = new();
    private Dictionary<int, Chunk> _chunks = new();
    private Queue<Chunk> _chunkPool = new();
    private Queue<Chunk> _loadedChunks = new();
    private Dictionary<BlockType, BlockTypeColor> _blockTypeColors = new();

    private int[,] _map;
    private int[,] _chunkMap;
    private int _previousChunkIndex;
    private int _oldestChunkIndex;
    private GameMode _gameMode => _mapLoader.GameMode;
    private bool _canLoadMap;
    private bool _isChunkMapSaved;
    private bool _isInitialLoading;
    private bool _isWaveFunctionEnd;

    public Dictionary<int, Chunk> Chunks => _chunks;
    public event Action OnInitialMapLoaded;

    private void Awake() => Init();
    private void Update() => UpdateStreaming();
    private void OnDestroy() => UnbindMapEvents();

    private void UpdateStreaming()
    {
        if (!_canLoadMap || _targetTransform == null) return;

        if (_loadedChunks.Count == 0 && !_isInitialLoading) StartCoroutine(InitialLoad());

        int currentChunkIndex = GetChunkIndex(_targetTransform.position);

        if (currentChunkIndex <= _previousChunkIndex) return;

        if (currentChunkIndex - RENDER_DISTANCE > _oldestChunkIndex)
        {
            RemoveOldestChunk();
            RemoveOldestInvisibleWalls();
        }

        _previousChunkIndex = currentChunkIndex;

        StartCoroutine(LoadChunkRoutine(currentChunkIndex + RENDER_DISTANCE));
        LoadInvisibleWall(currentChunkIndex + RENDER_DISTANCE);
    }

    private int GetChunkIndex(Vector3 position)
    {
        return (int)position.x / CHUNK_SIZE;
    }

    private void CreateChunkMap(int chunkIndex)
    {
        _waveFunction.StartWaveFunctionCollapse(chunkIndex);
    }

    private IEnumerator LoadChunkRoutine(int chunkIndex)
    {
        if (_chunkPool.Count == 0) yield break;

        Chunk chunk = _chunkPool.Dequeue();

        if (_gameMode == GameMode.Infinite)
        {
            CreateChunkMap(chunkIndex);
            yield return new WaitUntil(() => _isWaveFunctionEnd);
        }
        else
        {
            _chunkMap = WorldMapToChunkMap(chunkIndex);
        }

        if (!_chunks.ContainsKey(chunkIndex))
            _chunks.Add(chunkIndex, chunk);

        if (_chunkMap == null) yield break;

        _isWaveFunctionEnd = false;
        _isChunkMapSaved = true;

        chunk.SetChunkData(chunkIndex, _splineManager, _chunkMap, _blockTypeColors, _material, _textureSize, _atlasSize);
        chunk.gameObject.SetActive(true);

        _chunkMap = null;
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

    private IEnumerator InitialLoad()
    {
        _isInitialLoading = true;

        for (int i = 0; i <= RENDER_DISTANCE; i++)
        {
            StartCoroutine(LoadChunkRoutine(i));
            yield return new WaitUntil(() => _isChunkMapSaved);
            LoadInvisibleWall(i);

            _isChunkMapSaved = false;
        }

        _oldestChunkIndex = 0;
        OnInitialMapLoaded?.Invoke();
    }

    private void CreateChunkPool()
    {
        for (int i = 0; i < RENDER_DISTANCE * 2 + 1; i++)
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

        if (_map.GetLength(0) < chunkIndex * CHUNK_SIZE) return null;

        for (int y = 0; y < _map.GetLength(1); y++)
        {
            for (int x = chunkIndex * CHUNK_SIZE; x < (chunkIndex + 1) * CHUNK_SIZE; x++)
            {
                if (x >= _map.GetLength(0))
                {
                    chunkMap[x - chunkIndex * CHUNK_SIZE, y] = 0;
                }
                else
                {
                    chunkMap[x - chunkIndex * CHUNK_SIZE, y] = _map[x, y];
                }
            }
        }
        return chunkMap;
    }

    public Chunk GetChunk(Vector2Int coord)
    {
        return _chunks[coord.x / CHUNK_SIZE];
    }

    private void OnWaveFunctionEnd(int[,] chunkMap)
    {
        _chunkMap = chunkMap;
        _isWaveFunctionEnd = true;
    }

    private void OnMapSaved()
    {
        _canLoadMap = true;
        _map = _mapLoader.Map;
    }

    private void BindMapEvents()
    {
        if (_waveFunction != null && _gameMode == GameMode.Infinite)
        _waveFunction.OnWaveFunctionEnd += OnWaveFunctionEnd;
        _mapLoader.OnMapSaved += OnMapSaved;
    }

    private void UnbindMapEvents()
    {
        if (_waveFunction != null && _gameMode == GameMode.Infinite)
        _waveFunction.OnWaveFunctionEnd -= OnWaveFunctionEnd;
        _mapLoader.OnMapSaved -= OnMapSaved;
    }

    private void AddBlockColors()
    {
        _blockTypeColors[BlockType.Grass] = new BlockTypeColor(BlockType.Grass, new Dictionary<Vector2Int, float>());
        _blockTypeColors[BlockType.Grass].BlockColors.Add(new Vector2Int(0, 3), 0.1f);
        _blockTypeColors[BlockType.Grass].BlockColors.Add(new Vector2Int(1, 3), 0.9f);

        _blockTypeColors[BlockType.Tree] = new BlockTypeColor(BlockType.Tree, new Dictionary<Vector2Int, float>());
        _blockTypeColors[BlockType.Tree].BlockColors.Add(new Vector2Int(3, 3), 0.5f);
        _blockTypeColors[BlockType.Tree].BlockColors.Add(new Vector2Int(0, 2), 0.5f);

        _blockTypeColors[BlockType.Rock] = new BlockTypeColor(BlockType.Rock, new Dictionary<Vector2Int, float>());
        _blockTypeColors[BlockType.Rock].BlockColors.Add(new Vector2Int(1, 2), 0.5f);
        _blockTypeColors[BlockType.Rock].BlockColors.Add(new Vector2Int(2, 2), 0.5f);

        _blockTypeColors[BlockType.Water] = new BlockTypeColor(BlockType.Water, new Dictionary<Vector2Int, float>());
        _blockTypeColors[BlockType.Water].BlockColors.Add(new Vector2Int(3, 2), 1f);

        _blockTypeColors[BlockType.Obstacle] = new BlockTypeColor(BlockType.Obstacle, new Dictionary<Vector2Int, float>());
        _blockTypeColors[BlockType.Obstacle].BlockColors.Add(new Vector2Int(0, 1), 1f);
    }

    private void Init()
    {
        BindMapEvents();
        AddBlockColors();
        CreateChunkPool();
        CreateInvisibleWallPool();
    }
}
