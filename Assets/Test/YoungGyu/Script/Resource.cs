using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour, IInteractable, IPoolable
{
    public GameObject GameObject => gameObject; 
    public abstract BlockType BlockType { get; }
    [Header("Drop Settings")] 
    public  BlockType DropMaterialType;
    private Outline _outline;
    
    private bool _isMining = false;
    
    protected int Health = 3;
    
    private float _cooldownTimer = 0f;
    private const float COOLDOWN = 1f;
    [Header("Damage Visuals")]
    [SerializeField] private GameObject _visualTop;
    [SerializeField] private GameObject _visualMiddle;
    [SerializeField] private GameObject _visualBottom;

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
            Debug.Log("Resource : Mining End");
            OnMined();
        }
    }

    public virtual void Initialize()
    {
        Health = 3;
        _cooldownTimer = 0f;
        UpdateVisuals();
    }

    public virtual void ProcessMining()
    {
        _isMining = true;
        Debug.Log("Resource : ProcessMining");
        
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

    public abstract void BreakResource();

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
                ReturnToPool(GetComponent<IPoolable>());
                break;
        }
    }



    public void ReturnToPool(IPoolable poolable)
    {
        
        gameObject.SetActive(false);
        //풀에 다시 넣기
        ObjectPool.Instance.Return(poolable);
    }

    public abstract void AutoInteract(IInteractable interactable);


    public IInteractable ButtonInteract(IInteractable interactable)
    {
        return interactable;
    }

    public void Targeted()
    {
        _outline.enabled = true;
        Debug.Log("Targeted");
    }

    public void Untargeted()
    {
        _outline.enabled = false;
        _cooldownTimer = 0f;
        Debug.Log("Untargeted");
    }
}
