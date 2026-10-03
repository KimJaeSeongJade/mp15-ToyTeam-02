using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestObjectPool : MonoBehaviour
{
    private Stack<IPoolable> _poolables = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IPoolable poolable = ObjectPool.Instance.Take(BlockType.Axe);
            poolable.GameObject.transform.position = Vector3.zero;
            poolable.GameObject.SetActive(true);
            _poolables.Push(poolable);

            Debug.Log($"Got {poolable.GameObject.name} from object pool!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_poolables.Count == 0) Debug.Log("Poolables empty");

            IPoolable poolable = _poolables.Pop();
            ObjectPool.Instance.Return(poolable);
            poolable.GameObject.SetActive(false);
            poolable = null;

            Debug.Log($"Object returned!");
        }
    }
}
