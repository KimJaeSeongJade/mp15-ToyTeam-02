using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour, IInteractable, IPoolable
{
    public GameObject GameObject => gameObject; 
    public abstract BlockType BlockType { get; }
    [Header("Drop Settings")] 
    public BlockType DropMaterialType;
    
    protected int Health = 3;
    
    private float _cooldown = 0f;
    
    [Header("Damage Visuals")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;
    

    public virtual void Initialize()
    {
        Health = 3;
        _cooldown = 0f;
        UpdateVisuals();
    }

    public virtual void ProcessMining(float deltaTime)
    {
        _cooldown += deltaTime;
        if (_cooldown >= 1.0f)
        {
            _cooldown = 0f;
            OnMined();
        }
    }
    
    public virtual void OnMined()
    {
        Health--;
        UpdateVisuals();

        if (Health <= 0)
        {
            BreakResource();
        }
        
    }

    public virtual void BreakResource()
    {
        IPoolable dropItem = ObjectPool.Instance.Take(DropMaterialType);
        dropItem.GameObject.transform.position = transform.position;
        dropItem.GameObject.SetActive(true);
        
        ReturnToPool();
    }

    protected virtual void UpdateVisuals()
    {
        switch (Health)
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



    public void ReturnToPool()
    {
        
        gameObject.SetActive(false);
        //풀에 다시 넣기
        ObjectPool.Instance.Return(this);
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
        _cooldown = 0f;
    }
}
