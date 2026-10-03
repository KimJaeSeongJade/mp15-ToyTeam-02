using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Material : MonoBehaviour, IStackable, IPoolable, IInteractable
{
    [Header("Material Settings")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;

    public GameObject GameObject => gameObject;
    public abstract BlockType BlockType { get; }
    public abstract Outline Outline { get; }

    public int Count { get; protected set; }
    public int MaxStack => 3;
    
    
    private void OnEnable() => Init();

    private void Init()
    {
        Outline.enabled = false;
        Count = 1;
        UpdateVisuals();
    }
    
    public void AddCount(int amount)
    {
        Count += amount;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        switch (Count)
        {
            case 3:
                _visualTop.SetActive(true);
                _visualMiddle.SetActive(true);
                _visualBottom.SetActive(true);
                break;
            case 2:
                _visualTop.SetActive(false);
                break; 
            case 1:
                _visualTop.SetActive(false);
                _visualMiddle.SetActive(false);
                break;
            case 0:
                ReturnToPool();
                break;
        }
    }

    public void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == BlockType)
        {
            Material material = interactable as Material;

            int totalCount = Count + material.Count;

            if (totalCount > MaxStack)
            {
                material.Count = MaxStack;
                Count = totalCount - MaxStack;
            }
            else
            {
                material.Count = totalCount;
                Count = 0;
                ReturnToPool();
            }
        }
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
        Outline.enabled = false;
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(this);
    }
}

