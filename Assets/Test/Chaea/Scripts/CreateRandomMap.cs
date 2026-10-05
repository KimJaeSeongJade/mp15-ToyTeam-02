using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateRandomMap : MonoBehaviour
{
    private MapLoader _mapLoader;

    private int[,] _inputMap;
    private int[,] _outputMap;

    private void LoadInputMap(int[,] inputMap)
    {
        _inputMap = inputMap;
        int width = _inputMap.GetLength(0);
        int height = _inputMap.GetLength(1);
    }

    private void Setup()
    {
        _outputMap = new int[ChunkManager.CHUNK_SIZE, ChunkManager.CHUNK_SIZE];

    }
}

