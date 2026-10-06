using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock : MonoBehaviour, IPoolable
{
    public GameObject GameObject { get => gameObject; }

    public BlockType BlockType { get => BlockType.Obstacle; }

    public void ReturnToPool()
    {
        ObjectPool.Instance.Return(this);
    }
}
