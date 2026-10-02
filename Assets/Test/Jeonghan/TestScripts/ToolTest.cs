using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTest : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _selectedMaterial;

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
        gameObject.GetComponent<MeshRenderer>().material = _selectedMaterial;
    }

    public void Untargeted()
    {
        gameObject.GetComponent<MeshRenderer>().material = _baseMaterial;
    }
}
