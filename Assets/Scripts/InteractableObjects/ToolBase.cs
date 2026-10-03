using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour, IInteractable, IPoolable
{
    public GameObject GameObject => gameObject;
    public abstract BlockType BlockType { get; }
    public abstract Outline Outline { get; }

    private void OnEnable() => Init();

    private void Init()
    {
        Outline.enabled = false;
    }

    public void AutoInteract(IInteractable interactable)
    {
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
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
        ObjectPool.Instance.Return(this);
        gameObject.SetActive(false);
    }
}
