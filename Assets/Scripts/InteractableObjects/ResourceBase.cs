using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ResourceBase : MonoBehaviour, IInteractable, IPoolable
{
    [Header("Material Settings")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;
    [SerializeField] private AudioClip _hitSound;

    public GameObject GameObject => gameObject; 
    public abstract BlockType BlockType { get; }
    public abstract BlockType ToolType { get; }
    public abstract BlockType DropMaterialType { get; }
    public abstract Outline Outline { get; }

    private IPoolable _dropItem;

    public int Health { get; protected set; }

    private void OnEnable() => Init();

    private void Init()
    {
        Outline.enabled = false;
        Health = 3;
        UpdateVisuals();
    }
    
    public void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == ToolType)
        {
            OnMined();
        }
    }

    private void OnMined()
    {
        Health--;
        AudioPlayer hitAudioPlayer = AudioManager.Instance.Take();

        hitAudioPlayer
            .Init()
            .SetPriority(100)
            .SetClip(_hitSound)
            .SetLoop(false)
            .Play();
        UpdateVisuals();

        if (Health <= 0)
        {
            BreakResource();
        }
    }

    private void UpdateVisuals()
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
        }
    }

    private void BreakResource()
    {

       

       
        
        _dropItem = ObjectPool.Instance.Take(DropMaterialType);
        _dropItem.GameObject.transform.position = transform.position;
        _dropItem.GameObject.SetActive(true);

        ReturnToPool();
    }

    /// <summary>
    /// 플레이어가 버튼을 눌러 바닥에 있는 이 오브젝트와 상호작용
    /// </summary>
    /// <param name="interactable"> 플레이어 손에 있는 오브젝트 </param>
    /// <returns></returns>
    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return interactable;
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
