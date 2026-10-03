using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTrain : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get; }

    public BlockType BlockType { get; }

    public void AutoInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public IInteractable ButtonInteract(IInteractable interactable)
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
