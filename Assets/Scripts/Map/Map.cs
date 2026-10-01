using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance;

    private int[,] _worldMap;
    private IInteractable[,] _interactableMap;

    private void Awake() => SetSingleton();

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public BlockType GetBlockType(Vector2Int coord)
    {
        return (BlockType)_worldMap[coord.x, coord.y];
    }

    public IInteractable GetInteractable(Vector2Int coord)
    {
        return _interactableMap[coord.x, coord.y];
    }

    public void SetInteractable(Vector2Int coord, IInteractable interactable)
    {
        _interactableMap[coord.x, coord.y] = interactable;
    }

    public void SetMapData(int[,] mapData)
    {
        _worldMap = mapData;
    }
}