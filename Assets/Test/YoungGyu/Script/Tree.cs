using System;
using UnityEngine;

// 질문자님의 완벽한 뼈대인 'Resource'를 그대로 상속받습니다!
public class Tree : Resource
{
    public override BlockType BlockType => BlockType.Tree;
    private IPoolable _dropItem;

    public override void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == BlockType.Axe)
        {
            Debug.Log("Tree : AutoInteract");
            ProcessMining();
        }

    }

    protected override void Awake()
    {
        base.Awake();
        DropMaterialType = BlockType.Wood;

    }

    private void OnEnable()
    {
        Initialize();
    }
    
    public override void BreakResource()
    {
        _dropItem = ObjectPool.Instance.Take(DropMaterialType);
        _dropItem.GameObject.transform.position = transform.position;
        _dropItem.GameObject.SetActive(true);
        
        ReturnToPool(this);
        
    }
    
    
}