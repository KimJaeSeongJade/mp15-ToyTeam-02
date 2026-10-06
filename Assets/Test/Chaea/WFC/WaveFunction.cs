using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private float _neighborBonus = 3.0f;

    public int Dimensions => ChunkManager.CHUNK_SIZE;
    public List<Cell> GridComponents;
    public int Iterations = 0;
    public event Action<int[,]> OnWaveFunctionEnd;

    private Dictionary<int, Tile> _tiles = new();
    private bool _isInitialLoad = true;
    private int[,] _initialMap;

    private Vector2Int[] _directions = new[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    /// <summary>
    /// 파동 함수 붕괴 알고리즘을 실행하여 맵 생성 시작
    /// </summary>
    public void StartWaveFunctionCollapse()
    {
        Debug.Log("WaveFunctionCollapse started");
        int[,] sampleMapData = _mapLoader.Map;

        if (_isInitialLoad) LoadMapData(sampleMapData);
        GridComponents = new List<Cell>();
        InitializeGrid();
    }

    // 샘플 맵 데이터로 Weight, Rules 설정
    private void LoadMapData(int[,] mapData)
    {
        _tiles.Clear();

        // Weight 설정
        for (int y = 0; y < mapData.GetLength(1); y++)
        {
            for (int x = 0; x < mapData.GetLength(0); x++)
            {
                int blockType = mapData[x, y];

                if (!_tiles.TryGetValue(blockType, out Tile tile))
                {
                    tile = new Tile(blockType, 0f);
                    _tiles[blockType] = tile;
                }

                tile.Weight += 1f;
            }
        }

        // Rules 설정
        for (int y = 0; y < mapData.GetLength(1); y++)
        {
            for (int x = 0; x < mapData.GetLength(0); x++)
            {
                int currentType = mapData[x, y];
                Tile currentTile = _tiles[currentType];

                for (int i = 0; i < _directions.Length; i++)
                {
                    int nx = x + _directions[i].x;
                    int ny = y + _directions[i].y;

                    if (nx >= 0 && nx < mapData.GetLength(0) && ny >= 0 && ny < mapData.GetLength(1))
                    {
                        int neighborType = mapData[nx, ny];
                        currentTile.AddNeighbor((Direction)i, neighborType);
                    }
                }
            }
        }
    }

    private void InitializeGrid()
    {
        GridComponents.Clear();


        if (_isInitialLoad)
        {
            _initialMap = _mapLoader.InfiniteInitialMap;
            Iterations = _initialMap.GetLength(0) * _initialMap.GetLength(1);
        }
        else
        {
            Iterations = 0;
        }

        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                if (_isInitialLoad &&
                    _initialMap != null &&
                    x < _initialMap.GetLength(0) &&
                    y < _initialMap.GetLength(1)
                    )
                {
                    // 1, 2, 3, 4, 5를 제외한 경우는 샘플 맵에 입력하지 않았으므로 제거
                    int mapIndex = _initialMap[x, y];
                    if (mapIndex == 0 || mapIndex > 5) mapIndex = 1;

                    Cell newCell = new Cell();
                    newCell.CreateCell(true, new Tile[] { _tiles[mapIndex] });
                    GridComponents.Add(newCell);
                }
                else
                {
                    Cell newCell = new Cell();
                    newCell.CreateCell(false, _tiles.Values.ToArray());
                    GridComponents.Add(newCell);
                }
            }
        }

        StartCoroutine(CheckEntropy());
    }

    private IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(GridComponents);

        tempGrid.RemoveAll(c => c.Collapsed);

        tempGrid.Sort((a, b) => a.TileOptions.Length.CompareTo(b.TileOptions.Length));

        int arrLength = tempGrid[0].TileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].TileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.Collapsed = true;

        int gridIndex = GridComponents.IndexOf(cellToCollapse);
        int cellX = gridIndex % Dimensions;
        int cellY = gridIndex / Dimensions;

        Tile selectedTile = SelectTile(cellToCollapse, cellX, cellY);
        cellToCollapse.TileOptions = new Tile[] { selectedTile };

        UpdateGeneration();
    }

    private Tile SelectTile(Cell targetCell, int cellX, int cellY)
    {
        float totalWeight = 0f;
        Dictionary<Tile, float> adjustedWeights = new Dictionary<Tile, float>();


        foreach (Tile tile in targetCell.TileOptions)
        {
            float currentWeight = tile.Weight;
            int matchCount = 0;

            // 4방향 이웃 검사
            for (int i = 0; i < _directions.Length; i++)
            {
                int nx = cellX + _directions[i].x;
                int ny = cellY + _directions[i].y;

                if (nx >= 0 && nx < Dimensions && ny >= 0 && ny < Dimensions)
                {
                    Cell neighbor = GridComponents[nx + ny * Dimensions];

                    if (neighbor.Collapsed && neighbor.TileOptions.Length > 0)
                    {
                        if (neighbor.TileOptions[0].BlockType == tile.BlockType)
                        {
                            matchCount++;
                        }
                    }
                }
            }

            if (matchCount > 0)
            {
                currentWeight *= Mathf.Pow(_neighborBonus, matchCount);
            }

            adjustedWeights[tile] = currentWeight;
            totalWeight += currentWeight;
        }

        // 가중치 선택
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var pair in adjustedWeights)
        {
            cumulative += pair.Value;
            if (roll <= cumulative) return pair.Key;
        }

        return targetCell.TileOptions[0];
    }

    private void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(GridComponents);

        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                var index = x + y * Dimensions;
                if (GridComponents[index].Collapsed)
                {
                    // Debug.Log("called");
                    newGenerationCell[index] = GridComponents[index];
                }
                else
                {
                    List<Tile> options = _tiles.Values.ToList();

                    // Up
                    if (y < Dimensions - 1)
                    {
                        Cell up = GridComponents[x + (y + 1) * Dimensions];
                        CheckValidity(options, up.TileOptions, Direction.Down);
                    }

                    // Right
                    if (x < Dimensions - 1)
                    {
                        Cell right = GridComponents[x + 1 + y * Dimensions];
                        CheckValidity(options, right.TileOptions, Direction.Left);
                    }

                    // Down
                    if (y > 0)
                    {
                        Cell down = GridComponents[x + (y - 1) * Dimensions];
                        CheckValidity(options, down.TileOptions, Direction.Up);
                    }

                    // Left
                    if (x > 0)
                    {
                        Cell left = GridComponents[x - 1 + y * Dimensions];
                        CheckValidity(options, left.TileOptions, Direction.Right);
                    }

                    newGenerationCell[index].RecreateCell(options.ToArray());
                }
            }
        }

        GridComponents = newGenerationCell;
        Iterations++;

        if (Iterations < Dimensions * Dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
        else
        {
            Debug.Log("WFC Finished");
            OnWaveFunctionEnd?.Invoke(GetGeneratedMapData());
        }
    }

    private void CheckValidity(List<Tile> currentOptions, Tile[] neighborTileOptions, Direction oppositeDirection)
    {
        List<Tile> validOptions = new List<Tile>();

        foreach (Tile myTile in currentOptions)
        {
            bool isValid = false;

            foreach (Tile neighborTile in neighborTileOptions)
            {
                if (neighborTile.Rules[oppositeDirection].Contains(myTile.BlockType))
                {
                    isValid = true;
                    break;
                }
            }

            if (isValid)
            {
                validOptions.Add(myTile);
            }
        }

        currentOptions.Clear();
        currentOptions.AddRange(validOptions);
    }

    private int[,] GetGeneratedMapData()
    {
        int size = Dimensions;
        int[,] resultMap = new int[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = x + y * size;
                Cell cell = GridComponents[index];

                if (_isInitialLoad &&
                    _initialMap != null &&
                    x < _initialMap.GetLength(0) &&
                    y < _initialMap.GetLength(1)
                    )
                {
                    resultMap[x, y] = _initialMap[x, y];
                }

                else
                {
                    if (cell.Collapsed && cell.TileOptions != null && cell.TileOptions.Length > 0)
                    {
                        resultMap[x, y] = cell.TileOptions[0].BlockType;
                    }
                    else
                    {
                        Debug.Log("Collapsed failed");
                        resultMap[x, y] = 1;
                    }
                }
            }
        }

        _isInitialLoad = false;
        return resultMap;
    }
}