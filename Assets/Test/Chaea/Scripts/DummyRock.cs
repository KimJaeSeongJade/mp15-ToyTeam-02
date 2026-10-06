using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRock : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Rock;

    public void ReturnToPool()
    {
        ObjectPool.Instance.Return(this);
        gameObject.SetActive(false);
    }
}
