using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MaterialBase : MonoBehaviour, IStackable, IPoolable, IInteractable
{
    [Header("Material Settings")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;

    public GameObject GameObject => gameObject;
    public abstract BlockType BlockType { get; }
    public abstract Outline Outline { get; }

    [field: SerializeField] public int Count { get; protected set; }
    public int MaxStack => 3;
    
    
    private void OnEnable() => Init();

    private void Init()
    {
        Outline.enabled = false;
        Count = 1;
        UpdateStackVisuals();
    }
    
    public void AddCount(int amount)
    {
        Count += amount;
        UpdateStackVisuals();
    }

    private void UpdateStackVisuals()
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
                _visualMiddle.SetActive(true);
                _visualBottom.SetActive(true);
                break; 
            case 1:
                _visualTop.SetActive(false);
                _visualMiddle.SetActive(false);
                _visualBottom.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 플레이어가 raycast로 자동 상호작용 (재료 아이템 합쳐지기)
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == BlockType)
        {
            MaterialBase material = interactable as MaterialBase;

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

            MaterialBase materialBase = interactable as MaterialBase;
            materialBase.UpdateStackVisuals();
            UpdateStackVisuals();
        }
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        Map.Instance.SetHoldable(transform.position.WorldToCoord(), interactable);
        return this;
    }

    /// <summary>
    /// 플레이어가 타겟팅
    /// </summary>
    public void Targeted()
    {
        Outline.enabled = true;
    }

    /// <summary>
    /// 플레이어가 타겟팅 취소
    /// </summary>
    public void Untargeted()
    {
        Outline.enabled = false;
    }

    /// <summary>
    /// 오브젝트 풀로 반환
    /// </summary>
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(this);
    }
}

