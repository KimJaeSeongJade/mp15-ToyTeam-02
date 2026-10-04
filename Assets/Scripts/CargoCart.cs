using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoCart : Train
{
    /// <summary>
    /// 각 자원별 최대 보관 가능 수량
    /// </summary>
    [SerializeField] private int maxResourceCount = 3;
    
    /// <summary>
    /// 현재 보관 중인 목재 수량
    /// </summary>
    [SerializeField] private int currentWoodCount = 0;
    
    /// <summary>
    /// 현재 보관 중인 철 수량
    /// </summary> 
    [SerializeField] private int currentIronCount = 0;
    
    // 외부 참조용 프로퍼티 
    public int CurrentWoodCount => currentWoodCount;
    public int CurrentIronCount => currentIronCount;
    public int MaxResourceCount => maxResourceCount;

    protected virtual void Start()
    {
        // 앞 열차칸이 있다면 연결하고 초기화
        if (headCart != null)
        {
            Initialize(headCart);
        }

        // 2. 초기 3D 렌더러/시각적 자원 표시 갱신
        UpdateResourceVisual();
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
    /// 손에 든 아이템을 화물칸에 수납하고, 상호작용 후 플레이어 손에 남아아 할 재료를 반환합니다.
    /// </summary>
    /// <param name="inPlayerHand">플레이어가 투입하려는 아이템</param>
    /// <returns>수납 성공 시 null(빈손), 수납 실패 시 원래 들고 있던 아이템 유지</returns>
    private IInteractable PushResource(IInteractable inPlayerHand)
    {
        // 손에든 IInteractable 구현 객체를 재료 클래스로 전환해 변수에 담음
        MaterialBase material = inPlayerHand as MaterialBase;
        
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

                    // 전부 수납되었으므로 풀로 반환 후 빈손(null) 리턴
                    material.ReturnToPool();
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
                    
                    // 전부 수납되었으므로 풀로 반환 후 빈손(null) 리턴
                    material.ReturnToPool();
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
    /// <returns>꺼낸 자원 아이템 화쿨칸에 자원이 없다면 null</returns>
    private IInteractable GiveResource()
    {
        BlockType resourceToGive = BlockType.None;
        int popCount = 0;

        //  화물차에 목재가 1개라도있다면 목재를 먼저 꺼냄
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

        // 풀에서 자원 생성 후 플레이어 손에 넘겨줄 IInteractable 직접 반환
        return SpawnResourceItem(resourceToGive, popCount);
    }
  
    /// <summary>
    /// 오브젝트 풀에서 지정된 자원을 꺼내 화물칸 위치에 활성화하고,
    /// 꺼낸 수량에 맞추어 MaterialBase 수량 및 시각 연출을 적용한뒤 반환합니다.
    /// </summary>
    /// <param name="type">꺼낼 자원의 블록 종류 </param>
    /// <param name="amount">플레이어 손에 전달할 총 재료 갯수</param>
    /// <returns>생성되어 수량이 세팅된 자원의 IInteractable</returns>
    private IInteractable SpawnResourceItem(BlockType type, int amount)
    {
        // 싱글톤 오브젝트 풀에서 지정된 재료의 IPoolable 꺼내기
        IPoolable poolable = ObjectPool.Instance.Take(type);
        
        // 꺼낸 IPoolable 구현 객체를 MaterialBase 객체 변수에 담음
        MaterialBase material = poolable as MaterialBase;

        if (material != null)
        {
            // 화물칸 위치로 이동 후 오브젝트 활성화 
            material.transform.position = transform.position;
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
    /// 현재 보관량에 따라 화물칸 위에 쌓이는 3D 자원 모델을 켜고 끕니다.
    /// </summary>
    private void UpdateResourceVisual()
    {
        // TODO: 화물칸에 쌓인 재료 카운트에 맞는 비주얼 효과 구현
        // currentWoodCount, currentIronCount 수량에 맞춰 SetActive(true/false) 처리
    }

    
}