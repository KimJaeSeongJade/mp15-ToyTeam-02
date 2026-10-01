using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public const int CHUNK_SIZE = 20;

    [SerializeField] private Transform _targetTransform; // 청크 스트리밍 기준 타켓의 transform
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private Material _material;
    [SerializeField] private float _textureSize = 32f;
    [SerializeField] private int _atlasSize = 4;
    [SerializeField] private int _renderDistance = 2;

    private Queue<Chunk> _chunkPool = new();
    private Queue<Chunk> _loadedChunks = new();
    private int _previousChunkIndex;
    private int _oldestChunkIndex;
    private bool isInitialLoad;

    public bool canLoadMap => _mapLoader.CanLoadMap;

    private void Awake() => CreateChunkPool();
    private void Update() => UpdateStreaming();

    private void UpdateStreaming()
    {
        if (!canLoadMap || _targetTransform == null) return;

        if (_loadedChunks.Count == 0) InitialLoad();

        int currentChunkIndex = GetChunkIndex(_targetTransform.position);

        if (currentChunkIndex <= _previousChunkIndex) return;

        if (currentChunkIndex - _renderDistance > _oldestChunkIndex) RemoveOldestChunk();

        _previousChunkIndex = currentChunkIndex;
        LoadChunk(currentChunkIndex + _renderDistance);
    }

    private int GetChunkIndex(Vector3 position)
    {
        return (int)position.x / CHUNK_SIZE;
    }

    private void LoadChunk(int chunkIndex)
    {
        Chunk chunk = _chunkPool.Dequeue();

        chunk.SetChunkData(chunkIndex, _mapLoader.WorldMap, _material, _textureSize, _atlasSize);
        chunk.gameObject.SetActive(true);

        _loadedChunks.Enqueue(chunk);
    }

    private void RemoveOldestChunk()
    {
        Chunk chunk = _loadedChunks.Dequeue();

        chunk.gameObject.SetActive(false);
        _chunkPool.Enqueue(chunk);
    }

    private void InitialLoad()
    {
        for (int i = 0; i <= _renderDistance; i++)
        {
            LoadChunk(i);
        }

        _oldestChunkIndex = 0;
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
}
