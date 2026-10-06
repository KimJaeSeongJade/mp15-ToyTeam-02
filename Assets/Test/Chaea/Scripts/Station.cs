using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour, IPoolable
{
    public GameObject GameObject { get => gameObject; }

    public BlockType BlockType { get => BlockType.Station; }

    public void ReturnToPool()
    {
        ObjectPool.Instance.Return(this);
        gameObject.SetActive(false);
    }
}
