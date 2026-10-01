using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectPool : MonoBehaviour
{
    IPoolable _poolable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _poolable = ObjectPool.Instance.Take(BlockType.Rock);
            _poolable.GameObject.transform.position = Vector3.zero;
            _poolable.GameObject.SetActive(true);

            Debug.Log($"Got {_poolable.GameObject.name} from object pool!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ObjectPool.Instance.Return(_poolable);
            _poolable.GameObject.SetActive(false);
            _poolable = null;

            Debug.Log($"Object returned!");
        }
    }
}
