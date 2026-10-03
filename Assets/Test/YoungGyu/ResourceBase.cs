using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ResourceBase : MonoBehaviour, IInteractable, IPoolable
{
    private const float COOLDOWN = 1f;

    [Header("Material Settings")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;

    public GameObject GameObject => gameObject; 
    public abstract BlockType BlockType { get; }
    public abstract BlockType ToolType { get; }
    public abstract BlockType DropMaterialType { get; }
    public abstract Outline Outline { get; }

    private bool _isMining = false;
    private float _cooldownTimer = 0f;
    private IPoolable _dropItem;

    protected int Health = 3;

    private void OnEnable() => Init();
    private void Update() => Mining();

    private void Mining()
    {
        if (!_isMining)
        {
            return;
        }
        _cooldownTimer += Time.deltaTime;
        if (_cooldownTimer >= COOLDOWN)
        {
            _cooldownTimer = 0f;
            OnMined();
        }
    }

    private void Init()
    {
        Outline.enabled = false;
        Health = 3;
        _cooldownTimer = 0f;
        UpdateVisuals();
    }

    public void AutoInteract(IInteractable interactable)
    {
        if (interactable.BlockType == ToolType)
        {
            Debug.Log("RockResource : AutoInteract");
            ProcessMining();
        }
    }

    private void ProcessMining()
    {
        _isMining = true;
        Debug.Log("Resource : ProcessMining");
        
    }

    private void OnMined()
    {
        Health--;
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
            case 0:
                ReturnToPool();
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

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        ObjectPool.Instance.Return(this);
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return interactable;
    }

    public void Targeted()
    {
        Outline.enabled = true;
    }

    public void Untargeted()
    {
        Outline.enabled = false;
        _isMining = false;
        _cooldownTimer = 0f;
    }
}
