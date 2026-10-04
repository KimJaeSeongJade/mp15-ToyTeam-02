using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCargoCart : Train
{
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
    /// <param name="inPlayerHand">플레이어가 손에 들고 있는 아이템 </param>
    /// <returns>상호작용 후 플레이어의 손으로 넘겨줄 아이템</returns>
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
    /// <param name="inPlayerHand">플레이어가 투입하려는 아이템</param>
    /// <returns>수납 성공 시 null(빈손), 수납 실패 시 원래 들고 있던 아이템 유지</returns>
    private IInteractable PushResource(IInteractable inPlayerHand)
    {
        /* [테스트용 주석처리]
        // 손에든 IInteractable 구현 객체를 재료 클래스로 전환해 변수에 담음
        MaterialBase material = inPlayerHand as MaterialBase;
        if (material == null) return inPlayerHand;
        */
        
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

                    // 남은 재료 아이템을 플레이어 손에 돌려줌
                    return material;
                }
                // 1.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentWoodCount += material.Count;
                    UpdateResourceVisual();

                    // [테스트용 주석처리] 오브젝트 풀 반납 제외
                    // material.ReturnToPool();
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
                    
                    // 남은 재료 아이템을 플레이어 손에 돌려줌
                    return material; 
                }
                // 2.2) 플레이어가 소지한 재료를 전부 화물칸에 넣는 경우
                else
                {
                    currentIronCount += material.Count;
                    UpdateResourceVisual();
                    
                    // [테스트용 주석처리] 오브젝트 풀 반납 제외
                    // material.ReturnToPool();
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
    /// <returns>꺼낸 자원 아이템 (오브젝트 풀 제외 테스트로 null 반환)</returns>
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

        // [테스트용 수정] 오브젝트 풀을 사용하지 않으므로 SpawnResourceItem 호출 없이 인출 수량만 차감 후 null 반환
        Debug.Log($"[화물칸에서 꺼내기]: {resourceToGive} ({popCount}개)");
        return new TestHandData(resourceToGive, popCount);
        
        // [테스트용 주석처리]
        // return SpawnResourceItem(resourceToGive, popCount);
    }
  
    /// <summary>
    /// [오브젝트 풀 미사용으로 테스트용 주석 처리]
    /// </summary>
    private IInteractable SpawnResourceItem(BlockType type, int amount)
    {
        /*
        IPoolable poolable = ObjectPool.Instance.Take(type);
        MaterialBase material = poolable as MaterialBase;

        if (material != null)
        {
            material.transform.position = transform.position;
            material.gameObject.SetActive(true);

            if (amount > 1)
            {
                material.AddCount(amount - 1);
            }

            return material;
        }
        */
        return null;
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
    public override void Targeted()
    {
        // _outline 컴포넌트 null 가드 적용
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
}