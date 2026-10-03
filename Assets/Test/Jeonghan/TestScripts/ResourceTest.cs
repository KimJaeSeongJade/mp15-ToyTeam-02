using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTest : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEngine.Material _baseMaterial;
    [SerializeField] private UnityEngine.Material _selectedMaterial;

    public GameObject GameObject { get => gameObject; }
    public BlockType BlockType { get; }

    public void AutoInteract(IInteractable interactable)
    {
        Debug.Log("AutoInteract");
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return interactable;
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
