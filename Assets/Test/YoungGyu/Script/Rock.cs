using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Resource
{
    public override BlockType BlockType => BlockType.Rock;
    
    public override void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == BlockType.Pickaxe)
        {
            ProcessMining();
        }

    }

    protected override void Awake()
    { 
        base.Awake();
        DropMaterialType = BlockType.Iron;
        
    }

    private void OnEnable()
    {
        Initialize();
    }
    
    public override void BreakResource()
    {
        IPoolable dropItem = ObjectPool.Instance.Take(DropMaterialType);
        dropItem.GameObject.transform.position = transform.position;
        dropItem.GameObject.SetActive(true);
        
        ReturnToPool(this);
        
    }
    
    
}
