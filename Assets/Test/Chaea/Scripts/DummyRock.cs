using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRock : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Rock;

    public void ReturnToPool(IPoolable poolable)
    {
        throw new System.NotImplementedException();
    }
}
