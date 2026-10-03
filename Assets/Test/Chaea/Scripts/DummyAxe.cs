using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAxe : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Axe;

    public void ReturnToPool()
    {
        throw new System.NotImplementedException();
    }
}
