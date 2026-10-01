using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour, IInteractable, IPoolable
{
    public GameObject GameObject => gameObject; 
    public abstract BlockType BlockType { get; }
    [Header("Drop Settings")] 
    public BlockType _dropMaterialType;
    
    protected int Health = 3;
    
    private float _cooldown = 0f;
    
    [Header("Damage Visuals")]
    public GameObject visual1;
    public GameObject visual2;
    public GameObject visual3;
    

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
            _cooldown -= 1.0f;
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
        IPoolable dropItem = ObjectPool.Instance.Take(_dropMaterialType);
        dropItem.GameObject.transform.position = transform.position;
        dropItem.GameObject.SetActive(true);
        
        ReturnToPool(this);
    }

    protected virtual void UpdateVisuals()
    {
        if(visual1 != null) visual1.SetActive(Health == 3);
        if(visual2 != null) visual2.SetActive(Health == 2);
        if(visual3 != null) visual3.SetActive(Health == 1);
    }



    public void ReturnToPool(IPoolable poolable)
    {
        
        gameObject.SetActive(false);
        //풀에 다시 넣기
        ObjectPool.Instance.Return(poolable);
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
