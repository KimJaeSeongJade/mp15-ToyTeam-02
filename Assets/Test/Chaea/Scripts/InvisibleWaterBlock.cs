using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWaterBlock : MonoBehaviour, IPoolable
{
    public GameObject GameObject { get => gameObject; }

    public BlockType BlockType { get => BlockType.Water; }

    public void ReturnToPool()
    {
        ObjectPool.Instance.Return(this);
    }
}
