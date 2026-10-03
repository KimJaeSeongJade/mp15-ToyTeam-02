using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRail : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Rail;

    public void ReturnToPool()
    {
        throw new System.NotImplementedException();
    }
}
