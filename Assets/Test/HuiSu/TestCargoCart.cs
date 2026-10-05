using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCargoCart : Train, IInteractable
{
    [Header("제작칸 연결")]
    [SerializeField] private TestCraftCart targetCraftCart; // 자원을 전달할 제작칸 

    [Header("재료 보관량에 따른 카트 비주얼")]
    [SerializeField] private GameObject emptyCartVisual; // 비어있을 때 활성화할 메쉬
    [SerializeField] private GameObject fullCartVisual;  // 자원이 있을 때 활성화할 메쉬
    
    [Header("재료 보유량")]
    [SerializeField] private int maxResourceCount = 3;
    [SerializeField] private int currentWoodCount = 0;
    [SerializeField] private int currentIronCount = 0;
    
    // 외부 참조용 프로퍼티 
    public int CurrentWoodCount => currentWoodCount;
    public int CurrentIronCount => currentIronCount;
    public int MaxResourceCount => maxResourceCount;

    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.None;

    private Outline _outline;

    private void Awake()
    {
        CacheComponents();
    }

    public void Start()
    {
        // 화물차의 비주얼 상태 초기화
        UpdateResourceVisual();
        
        // 테두리 끄기
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }
    
    private void CacheComponents()
    {
        _outline = GetComponent<Outline>();
    }

    /// <summary>
    /// 플레이어와 상호작용하여 재료를 화물칸에 투입하거나 빈손일 경우 재료를 꺼냅니다
    /// </summary>
    /// <param name="inPlayerHand">플레이어가 손에 들고 있는 아이템 </param>
    /// <returns>상호작용 후 플레이어의 손으로 넘겨줄 아이템</returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        // 1. 플레이어가 손에 재료를 들고 있다면 아이템을 화물칸에 투입
        if (inPlayerHand != null)
        {
            return PushResource(inPlayerHand);
        }

        // 2. 플레이어가 빈손인 경우 화물칸에서 아이템을 꺼냄
        return GiveResource();
    }
    
    /// <summary>
    /// 손에 든 아이템을 화물칸에 수납하고, 상호작용 후 플레이어 손에 남아있어야 할 재료를 반환합니다.
    /// </summary>
    /// <param name="inPlayerHand">플레이어가 투입하려는 아이템</param>
    /// <returns>수납 성공 시 null(빈손), 수납 실패 시 원래 들고 있던 아이템 유지</returns>
    private IInteractable PushResource(IInteractable inPlayerHand)
    {
        // [테스트용 임시 재료] 
        TestHandData material = inPlayerHand as TestHandData;
        
        if (material == null)
        {
            return inPlayerHand;
        }
        
        // 1. 목재 수납
        if (material.BlockType == BlockType.Wood)
        {
            // 목재를 저장할 공간이 남아있다면 
            if (currentWoodCount < maxResourceCount)
            {
                // 화물칸에 더 담을 수 있는 갯수 계산
                int remainCount = maxResourceCount - currentWoodCount;
            
                // 1.1) 손에든 재료의 갯수가 화물칸의 남은 공간보다 큰 경우
                if (material.Count > remainCount)
                {
                    // 화물칸은 가득 채우고
                    currentWoodCount = maxResourceCount;
                    // 화물칸을 채우고 남은 갯수만큼 플레이어가 소지한 재료 갯수 차감 
                    material.AddCount(-remainCount);
                    UpdateResourceVisual();

                    // 자원이 투입되었으므로 제작칸에 제작 시도
                    CallCraftCart();

                    // 남은 재료 아이템을 플레이어 손에 돌려줌
                    return material;
                }
                // 1.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentWoodCount += material.Count;
                    UpdateResourceVisual();

                    // 자원이 투입되었으므로 제작칸에서 제작 시도
                    CallCraftCart();

                    return null;
                }
            }
        }
        // 2. 철 수납
        else if (material.BlockType == BlockType.Iron)
        {
            // 철을 저장할 공간이 남아있다면 
            if (currentIronCount < maxResourceCount)
            {
                // 화물칸에 더 담을 수 있는 갯수 계산
                int remainSpace = maxResourceCount - currentIronCount;

                // 2.1) 손에든 재료의 갯수가 화물칸의 남은 공간보다 큰 경우
                if (material.Count > remainSpace)
                {
                    // 화물칸은 가득 채우고
                    currentIronCount = maxResourceCount;
                    // 화물칸을 채우고 남은 갯수만큼 플레이어가 소지한 재료 갯수 차감 
                    material.AddCount(-remainSpace);
                    UpdateResourceVisual();

                    // 자원이 투입되었으므로 제작칸에 제작 시도
                    CallCraftCart();
                    
                    // 남은 재료 아이템을 플레이어 손에 돌려줌
                    return material; 
                }
                // 2.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentIronCount += material.Count;
                    UpdateResourceVisual();

                    // 자원이 투입되었으므로 제작칸에 제작 시도
                    CallCraftCart();
                    
                    return null;
                }
            }
        }

        // 화물칸 용량이 가득 찬 경우 플레이어의 자원 유지
        return inPlayerHand;
    }

    /// <summary>
    /// 빈손인 플레이어에게 화물칸의 재료를 꺼내어 전달
    /// </summary>
    /// <returns>꺼낸 자원 아이템</returns>
    private IInteractable GiveResource()
    {
        BlockType resourceToGive = BlockType.None;
        int popCount = 0;

        // 1. 화물차에 목재가 1개라도있다면 목재를 먼저 꺼냄
        if (currentWoodCount > 0)
        {
            resourceToGive = BlockType.Wood;
            popCount = currentWoodCount;
            currentWoodCount = 0;
        }
        // 2. 철(Iron) 인출
        else if (currentIronCount > 0)
        {
            resourceToGive = BlockType.Iron;
            popCount = currentIronCount;
            currentIronCount = 0;
        }

        // 꺼낼 자원이 전혀 없으면 빈손(null) 반환
        if (resourceToGive == BlockType.None) return null;

        UpdateResourceVisual();

        Debug.Log($"[TestCargoCart] 화물칸에서 자원 인출: {resourceToGive} ({popCount}개)");
        return new TestHandData(resourceToGive, popCount);
    }

    /// <summary>
    /// 제작칸에서 레일 제작 시 자원을 차감할 때 직접 호출합니다.
    /// </summary>
    public void ConsumeResources(int woodAmount, int ironAmount)
    {
        currentWoodCount -= woodAmount;
        if (currentWoodCount < 0) currentWoodCount = 0;

        currentIronCount -= ironAmount;
        if (currentIronCount < 0) currentIronCount = 0;

        Debug.Log($"[TestCargoCart] 자원 소모 완료 - 남은 목재: {currentWoodCount}, 남은 철: {currentIronCount}");
        UpdateResourceVisual();
    }

    /// <summary>
    /// 연결된 제작칸이 있는 경우 레일 제작 프로세스를 시도합니다.
    /// </summary>
    private void CallCraftCart()
    {
        if (targetCraftCart != null)
        {
            targetCraftCart.TryCraft();
        }
    }
    
    /// <summary>
    /// 현재 보관량에 따라 화물칸 위에 쌓이는 3D 자원 모델을 켜고 끕니다.
    /// </summary>
    private void UpdateResourceVisual()
    {
        // 보관 중인 전체 자원 수량 계산
        int totalResourceCount = currentWoodCount + currentIronCount;
        
        if (totalResourceCount > 0)
        {
            if (fullCartVisual != null) fullCartVisual.SetActive(true);
            if (emptyCartVisual != null) emptyCartVisual.SetActive(false);
        }
        else 
        {
            if (fullCartVisual != null) fullCartVisual.SetActive(false);
            if (emptyCartVisual != null) emptyCartVisual.SetActive(true);
        }
    }
    
    /// <summary>
    /// 플레이어가 접근하여 상호작용 가능한 경우 테두리 표시
    /// </summary>
    public void Targeted()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }
    
    public void Untargeted()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }

    public void AutoInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }
}