using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.Progress;

public class WaveFunction : MonoBehaviour
{
    public int Dimensions => ChunkManager.CHUNK_SIZE;
    public List<Cell> GridComponents;
    public Cell CellObj;

    public int Iterations = 0;

    private Dictionary<int, Tile> _tiles = new();

    private Vector2Int[] _directions = new[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private void Awake()
    {
        int[,] sampleMapData = new int[20, 40];

        LoadMapData(sampleMapData);

        GridComponents = new List<Cell>();
        InitializeGrid();
    }

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
                        int neighborType = mapData[ny, nx];
                        currentTile.AddNeighbor((Direction)i, neighborType);
                    }
                }
            }
        }
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                Cell newCell = new Cell();
                newCell.CreateCell(false, _tiles.Values.ToArray());
                GridComponents.Add(newCell);
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

        Tile selectedTile = SelectTile(cellToCollapse.TileOptions);
        cellToCollapse.TileOptions = new Tile[] { selectedTile };

        UpdateGeneration();
    }

    private Tile SelectTile(Tile[] tileOptions)
    {
        float totalWeight = 0f;
        foreach (Tile tile in tileOptions)
        {
            totalWeight += tile.Weight;
        }

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (Tile tile in tileOptions)
        {
            cumulative += tile.Weight;
            if (roll <= cumulative) return tile;
        }

        return tileOptions[0];
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
                    Debug.Log("called");
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
                    if (y < Dimensions - 1)
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
}