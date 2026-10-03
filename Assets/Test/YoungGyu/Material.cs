using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MaterialBase : MonoBehaviour, IStackable, IPoolable, IInteractable
{ 
    public GameObject GameObject => gameObject;
    public abstract BlockType BlockType { get; }


    public int Count { get; protected set; }
    public int MaxStack => 3;
    
    [Header("Material Settings")]
    public GameObject visual1;
    public GameObject visual2;
    public GameObject visual3;

    public virtual void Initialize(int count = 1)
    {
        Count = count;
        UpdateVisuals();

    }
    
    public void AddCount(int amount)
    {
        Count += amount;
        UpdateVisuals();
    }

    protected virtual void UpdateVisuals()
    {
        if(visual1 != null) visual1.SetActive(Count == 1);
        if (visual2 != null) visual2.SetActive(Count == 2);
        if (visual3 != null) visual3.SetActive(Count == 3);
    }
    public void AutoInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return this;
    }

    public void Targeted()
    {
        throw new System.NotImplementedException();
    }

    public void Untargeted()
    {
        throw new System.NotImplementedException();
    }

    public void ReturnToPool(IPoolable poolable)
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(this);
    }
     
}

