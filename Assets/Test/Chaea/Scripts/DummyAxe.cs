using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAxe : MonoBehaviour, IPoolable, IInteractable
{
    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.Axe;

    public void AutoInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public void ReturnToPool()
    {
        throw new System.NotImplementedException();
    }

    public void Targeted()
    {
        throw new System.NotImplementedException();
    }

    public void Untargeted()
    {
        throw new System.NotImplementedException();
    }
}
