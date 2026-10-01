using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailItem : MonoBehaviour, IInteractable, IStackable, IPoolable
{
    private int currentCount = 1;
    private int maxStack = 3;
    
    // true라면 타일로써의 레일, flase라면 아이템으로써의 레일
    [SerializeField] private bool isRailWay = false;
    private Outline _outline;
    
    /// <summary>
    /// 자신의 게임 오브젝트
    /// </summary>
    public GameObject GameObject { get; }
    
    /// <summary>
    /// 자신의 블록 종류
    /// </summary>
    public BlockType BlockType { get; }
    
    /// <summary>
    /// 쌓인 개수
    /// </summary>
    public int Count => currentCount;
    
    /// <summary>
    /// 최대로 소지 가능한 개수
    /// </summary>
    public int MaxStack => maxStack;

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        Init();
    }

    private void CacheComponents()
    {
        _outline = GetComponent<Outline>();
    }

    private void Init()
    {
        _outline.enabled = false;
    }

    /// <summary>
    /// 플레이어가 raycast로 자동 상호작용
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable interactable)
    {
        // 플레이어가 레일을 들고있을때 호출받음
    }

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable ButtonInteract(IInteractable interactable)
    {
        // 플레이어가 상호작용 버튼을 누를떄 호출 받음
        
        return interactable;
    }

    /// <summary>
    /// 자신의 외곽선 표시 활성화
    /// </summary>
    public void Targeted()
    {
        // 플레이어 감지범위내에 레일이 있을때 호출 받음
        _outline.enabled = true;
    }

    /// <summary>
    /// 자신의 외곽선 표시 비활성화
    /// </summary>
    public void Untargeted()
    {
        // 플레이어 감지범위를 벋어나면 호출 받음
        _outline.enabled = false;
    }

    /// <summary>
    /// 자기 자신을 오브젝트 풀로 반환
    /// </summary>
    /// <param name="poolable"> 자기 자신의 IPoolable </param>
    public void ReturnToPool(IPoolable poolable)
    {
        
    }
    
}
