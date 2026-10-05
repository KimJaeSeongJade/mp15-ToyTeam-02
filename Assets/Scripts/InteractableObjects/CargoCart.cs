using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoCart : Train
{
    [Header("연결할 제작칸")]
    [SerializeField] private CraftCart targetCraftCart;  // 자원을 전달할 제작칸

    [Header("상태에 따른 화물칸 메쉬")]
    [SerializeField] private GameObject emptyCartVisual; // 비어있을 때 활성화할 메쉬
    [SerializeField] private GameObject fullCartVisual;  // 자원이 있을 때 활성화할 메쉬
    
    [Header("화물 보관 설정")]
    [SerializeField] private int maxResourceCount = 3;   // 각 자원별 최대 보관 가능 수량
    [SerializeField] private int currentWoodCount = 0;   // 현재 보관 중인 목재 수량
    [SerializeField] private int currentIronCount = 0;   // 현재 보관 중인 철 수량
    
    // 외부 참조용 프로퍼티 
    public int CurrentWoodCount => currentWoodCount;
    public int CurrentIronCount => currentIronCount;
    public int MaxResourceCount => maxResourceCount;
    
    private Outline _outline;

    private void Awake()
    {
        CacheComponents();
    }

    public void Start()
    {
        // 앞 열차칸이 있다면 연결하고 초기화
        if (headCart != null)
        {
            Initialize(headCart);
        }

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
    public override IInteractable ButtonInteract(IInteractable inPlayerHand)
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
    private IInteractable PushResource(IInteractable inPlayerHand)
    {
        // 손에 든 아이템이 MaterialBase 타입이 아니면 그대로 반환 (예외 방지)
        MaterialBase material = inPlayerHand as MaterialBase;
        if (material == null)
        {
            return inPlayerHand;
        }
        
        // 1. 목재 수납
        if (material.BlockType == BlockType.Wood)
        {
            if (currentWoodCount < maxResourceCount)
            {
                int remainCount = maxResourceCount - currentWoodCount;
            
                // 1.1) 손에 든 재료의 개수가 화물칸의 남은 공간보다 큰 경우
                if (material.Count > remainCount)
                {
                    currentWoodCount = maxResourceCount;
                    material.AddCount(-remainCount);
                    UpdateResourceVisual();

                    // 자원이 수납되었으므로 제작칸 제작 시도
                    TryStartTargetCraft();

                    return material;
                }
                // 1.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentWoodCount += material.Count;
                    UpdateResourceVisual();

                    material.ReturnToPool();

                    // 자원이 수납되었으므로 제작칸 제작 시도
                    TryStartTargetCraft();

                    return null;
                }
            }
        }
        // 2. 철 수납
        else if (material.BlockType == BlockType.Iron)
        {
            if (currentIronCount < maxResourceCount)
            {
                int remainSpace = maxResourceCount - currentIronCount;

                // 2.1) 손에 든 재료의 개수가 화물칸의 남은 공간보다 큰 경우
                if (material.Count > remainSpace)
                {
                    currentIronCount = maxResourceCount;
                    material.AddCount(-remainSpace);
                    UpdateResourceVisual();

                    // 자원이 수납되었으므로 제작칸 제작 시도
                    TryStartTargetCraft();

                    return material; 
                }
                // 2.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentIronCount += material.Count;
                    UpdateResourceVisual();

                    material.ReturnToPool();

                    // 자원이 수납되었으므로 제작칸 제작 시도
                    TryStartTargetCraft();

                    return null;
                }
            }
        }

        // 화물칸 용량이 가득 차서 수납되지 않은 경우
        return inPlayerHand;
    }

    /// <summary>
    /// 빈손인 플레이어에게 화물칸의 재료를 꺼내어 전달
    /// </summary>
    private IInteractable GiveResource()
    {
        BlockType resourceToGive = BlockType.None;
        int popCount = 0;

        // 1. 화물차에 목재가 1개라도 있다면 목재를 먼저 꺼냄
        if (currentWoodCount > 0)
        {
            resourceToGive = BlockType.Wood;
            popCount = currentWoodCount;
            currentWoodCount = 0;
        }
        // 2. 철 꺼내기
        else if (currentIronCount > 0)
        {
            resourceToGive = BlockType.Iron;
            popCount = currentIronCount;
            currentIronCount = 0;
        }

        // 꺼낼 자원이 전혀 없으면 빈손(null) 반환
        if (resourceToGive == BlockType.None) return null;

        UpdateResourceVisual();

        // 풀에서 자원 생성 후 플레이어 손으로 전달
        return SpawnResourceItem(resourceToGive, popCount);
    }
  
    /// <summary>
    /// 오브젝트 풀에서 지정된 자원을 꺼내 활성화하고,
    /// 꺼낸 수량에 맞추어 MaterialBase 수량을 세팅한 뒤 반환합니다.
    /// </summary>
    private IInteractable SpawnResourceItem(BlockType type, int amount)
    {
        IPoolable poolable = ObjectPool.Instance.Take(type);
        MaterialBase material = poolable as MaterialBase;

        if (material != null)
        {
            material.gameObject.SetActive(true);

            // 기본 1개 외에 추가 인출 수량이 있다면 AddCount로 세팅 
            if (amount > 1)
            {
                material.AddCount(amount - 1);
            }

            return material;
        }

        return null;
    }
    
    /// <summary>
    /// 연결된 제작칸이 있다면 레일 제작 프로세스를 시도합니다.
    /// </summary>
    private void TryStartTargetCraft()
    {
        if (targetCraftCart != null)
        {
            targetCraftCart.TryCraft();
        }
    }

    /// <summary>
    /// 현재 보관량에 따라 화물칸 위에 쌓이는 모습을 바꿈
    /// </summary>
    private void UpdateResourceVisual()
    {
        int totalResourceCount = currentWoodCount + currentIronCount;
        
        if (totalResourceCount > 0)
        {
            fullCartVisual.SetActive(true);
            emptyCartVisual.SetActive(false);
        }
        else 
        {
            fullCartVisual.SetActive(false);
            emptyCartVisual.SetActive(true);
        }
    }
    
    public override void Targeted()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }
    
    public override void Untargeted()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }
    
    /// <summary>
    /// 제작칸에서 자원을 차감할 때 직접 호출
    /// </summary>
    public void ConsumeResources(int woodAmount, int ironAmount)
    {
        currentWoodCount -= woodAmount;
        if (currentWoodCount < 0)
        {
            currentWoodCount = 0;
        }

        currentIronCount -= ironAmount;
        if (currentIronCount < 0)
        {
            currentIronCount = 0;
        }

        UpdateResourceVisual();
    }
}