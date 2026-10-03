using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour, IInteractable
{
    public GameObject GameObject { get; }
    public abstract BlockType BlockType { get; }
    public abstract Outline Outline { get; }

    private void OnEnable() => Init();

    private void Init()
    {
        Outline.enabled = false;
    }

    public void AutoInteract(IInteractable interactable)
    {
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return this;
    }

    public void Targeted()
    {
        Outline.enabled = true;
    }

    public void Untargeted()
    {
        Outline.enabled = true;
    }
}
