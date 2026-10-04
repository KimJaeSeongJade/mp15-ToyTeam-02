using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WaveFunctionPath : MonoBehaviour
{
    public int Dimensions;
    public TilePath[] TileObjects;
    public List<CellPath> GridComponents;
    public CellPath CellObj;

    public int _iterations = 0;

    private void Awake()
    {
        GridComponents = new List<CellPath>();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < Dimensions; y++)
        {
            for (int x = 0; x < Dimensions; x++)
            {
                CellPath newCell = Instantiate(CellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, TileObjects);
                GridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    private IEnumerator CheckEntropy()
    {
        List<CellPath> tempGrid = new List<CellPath>(GridComponents);

        tempGrid.RemoveAll(c => c.Collapsed);

        tempGrid.Sort((a, b) => { return a.TileOptions.Length - b.TileOptions.Length; });

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

    private void CollapseCell(List<CellPath> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        CellPath cellToCollapse = tempGrid[randIndex];

        cellToCollapse.Collapsed = true;
        TilePath selectedTile = cellToCollapse.TileOptions[UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length)];
        cellToCollapse.TileOptions = new TilePath[] { selectedTile };

        TilePath foundTile = cellToCollapse.TileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);

        UpdateGeneration();
    }

    private void UpdateGeneration()
    {
        List<CellPath> newGenerationCell = new List<CellPath>(GridComponents);

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
                    List<TilePath> options = new List<TilePath>();
                    foreach (TilePath t in TileObjects)
                    {
                        options.Add(t);
                    }

                    //update above
                    if (y > 0)
                    {
                        CellPath up = GridComponents[x + (y - 1) * Dimensions];
                        List<TilePath> validOptions = new List<TilePath>();

                        foreach (TilePath possibleOptions in up.TileOptions)
                        {
                            var valOption = Array.FindIndex(TileObjects, obj => obj == possibleOptions);
                            var valid = TileObjects[valOption].UpNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //update right
                    if (x < Dimensions - 1)
                    {
                        CellPath right = GridComponents[x + 1 + y * Dimensions];
                        List<TilePath> validOptions = new List<TilePath>();

                        foreach (TilePath possibleOptions in right.TileOptions)
                        {
                            var valOption = Array.FindIndex(TileObjects, obj => obj == possibleOptions);
                            var valid = TileObjects[valOption].LeftNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look down
                    if (y < Dimensions - 1)
                    {
                        CellPath down = GridComponents[x + (y + 1) * Dimensions];
                        List<TilePath> validOptions = new List<TilePath>();

                        foreach (TilePath possibleOptions in down.TileOptions)
                        {
                            var valOption = Array.FindIndex(TileObjects, obj => obj == possibleOptions);
                            var valid = TileObjects[valOption].DownNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //look left
                    if (x > 0)
                    {
                        CellPath left = GridComponents[x - 1 + y * Dimensions];
                        List<TilePath> validOptions = new List<TilePath>();

                        foreach (TilePath possibleOptions in left.TileOptions)
                        {
                            var valOption = Array.FindIndex(TileObjects, obj => obj == possibleOptions);
                            var valid = TileObjects[valOption].RightNeighbors;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    TilePath[] newTileList = new TilePath[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        GridComponents = newGenerationCell;
        _iterations++;

        if (_iterations < Dimensions * Dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }

    private void CheckValidity(List<TilePath> optionList, List<TilePath> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}