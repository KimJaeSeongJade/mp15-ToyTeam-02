using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get; }
    public abstract BlockType BlockType { get; }
    
    
    
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
