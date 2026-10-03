using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// 오브젝트 풀 싱글톤
    /// </summary>
    public static ObjectPool Instance;

    [SerializeField] private List<GameObject> _prefabList;
    private Dictionary<BlockType, Stack<IPoolable>> _objectPoolDict = new();
    private Dictionary<BlockType, GameObject> _prefabDict = new();
    private Dictionary<BlockType, GameObject> _poolParent = new();

    private const int INITIAL_POOL_SIZE = 5;

    private void Awake() => SetSingleton();
    private void Start() => CreatePool();

    /// <summary>
    /// 오브젝트 풀에서 특정 블록 종류의 IPoolable 꺼내기
    /// </summary>
    /// <param name="blockType"> 블록 종류 </param>
    /// <returns></returns>
    public IPoolable Take(BlockType blockType)
    {
        if (_objectPoolDict.ContainsKey(blockType))
        {
            Stack<IPoolable> stack = _objectPoolDict[blockType];

            if (stack.Count > 0)
            {
                return _objectPoolDict[blockType].Pop();
            }
            else
            {
                GameObject newPoolable = Instantiate(_prefabDict[blockType], _poolParent[blockType].transform);
                newPoolable.SetActive(false);
                return newPoolable.GetComponent<IPoolable>();
            }
        }

        return null;
    }

    /// <summary>
    /// IPoolable 오브젝트를 오브젝트 풀에 반환
    /// </summary>
    /// <param name="poolable"> IPoolable 오브젝트 </param>
    public void Return(IPoolable poolable)
    {
        if (!_objectPoolDict.ContainsKey(poolable.BlockType)) return;

        _objectPoolDict[poolable.BlockType].Push(poolable);
    }

    private void CreatePool()
    {
        foreach (GameObject prefab in _prefabList)
        {
            Stack<IPoolable> objectPoolStack = new(INITIAL_POOL_SIZE);

            IPoolable poolable = prefab.GetComponent<IPoolable>();

            _objectPoolDict[poolable.BlockType] = objectPoolStack;
            _prefabDict[poolable.BlockType] = prefab;

            GameObject newPoolParent = new GameObject();
            newPoolParent.name = $"{prefab.name} Pool";
            newPoolParent.transform.SetParent(transform);
            _poolParent[poolable.BlockType] = newPoolParent;

            for (int i = 0; i < INITIAL_POOL_SIZE; i++)
            {
                GameObject newObject = Instantiate(prefab, newPoolParent.transform);
                newObject.SetActive(false);

                objectPoolStack.Push(newObject.GetComponent<IPoolable>());
            }
        }
    }

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
}
