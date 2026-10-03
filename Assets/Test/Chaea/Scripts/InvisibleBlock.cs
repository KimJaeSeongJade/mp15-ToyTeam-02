using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour, IPoolable
{
    public GameObject GameObject { get => gameObject; }

    public BlockType BlockType { get => BlockType.None; }

    public void ReturnToPool()
    {
        ObjectPool.Instance.Return(this);
    }
}
