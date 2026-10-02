using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MaterialBase : MonoBehaviour, IStackable, IPoolable, IInteractable
{ 
    public GameObject GameObject => gameObject;
    public abstract BlockType BlockType { get; }
    private Outline _outline;


    public int Count { get; protected set; }
    public int MaxStack => 3;
    
    [Header("Material Settings")]
    [SerializeField] public GameObject _visualTop;
    [SerializeField] public GameObject _visualMiddle;
    [SerializeField] public GameObject _visualBottom;

    
    protected virtual void Awake() => CacheComponents();

    public void CacheComponents()
    {
        _outline = GetComponentInChildren<Outline>();
    }

    protected virtual void  Start() => Init();


    public void Init()
    {
        _outline.enabled = false;
    }
    
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
                ReturnToPool(GetComponent<IPoolable>());
                break;
        }
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
        _outline.enabled = true;

    }

    public void Untargeted()
    {
        _outline.enabled = false;
    }

    public void ReturnToPool(IPoolable poolable)
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(poolable);
    }
     
}

