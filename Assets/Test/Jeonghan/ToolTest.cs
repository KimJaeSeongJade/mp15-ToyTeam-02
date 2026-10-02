using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTest : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get => gameObject; }
    public BlockType BlockType { get; }

    public void AutoInteract(IInteractable interactable)
    {
        
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return this;
    }

    public void Targeted()
    {
        Debug.Log($"{this} Untargeted");

    }

    public void Untargeted()
    {
       Debug.Log($"{this} Untargeted");
    }
}
