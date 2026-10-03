using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTestPlayer : MonoBehaviour, IInteractable
{
    [SerializeField] private ResourceTree _tree;
    [SerializeField] private ToolAxe _axe;
    
    private void Intteract()
    {
        Debug.Log("MaterialTestPlayer : Intteract");
        Debug.Log(_axe);
        _tree.AutoInteract(_axe.GetComponent<IInteractable>());
    }
    
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
        Intteract();
    }

    public void Untargeted()
    {
        throw new System.NotImplementedException();
    }
}
