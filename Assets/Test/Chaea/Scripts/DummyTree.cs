using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTree : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Tree;

    public void ReturnToPool(IPoolable poolable)
    {
        throw new System.NotImplementedException();
    }
}
