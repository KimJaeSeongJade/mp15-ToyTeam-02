using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCraftCart : Train, IInteractable
{
    [Header("화물칸 연결")]
    [SerializeField] private TestCargoCart targetCargoCart;    // 자원을 가져올 화물칸 (테스트용)

    [Header("상태에 따른 제작칸 메쉬")]
    [SerializeField] private GameObject emptyCartVisual;   // 대기 상태 시 활성화할 메쉬
    [SerializeField] private GameObject fullCartVisual;    // 레일 보유 시 활성화할 메쉬

    [Header("제작 설정")]
    [SerializeField] private float craftTime = 3.0f;       // 레일 1개 제작 소요 시간
    [SerializeField] private int maxRailStorage = 3;       // 최대 레일 보관 개수 
    [SerializeField] private int currentCraftCount = 0;    // 현재 보관중인 레일 개수

    // 외부 참조용 프로퍼티
    public int CurrentCraftCount => currentCraftCount;
    public bool IsCrafting => isCrafting;

    public GameObject GameObject => gameObject;

    public BlockType BlockType => BlockType.None;

    private bool isCrafting = false;
    
    private Outline _outline;

    private void Awake()
    {
        CacheComponents();
    }
    
    
    public void Start()
    {
        // 제작칸의 비주얼 상태 초기화
        UpdateCraftVisual();
        
        // 테두리 끄기
        if (_outline != null)
        {
            _outline.enabled = false;
        }

        Debug.Log($"[TestCraftCart] 초기화 완료 - 보관된 레일 수: {currentCraftCount}/{maxRailStorage}");

        // 게임 시작 시 화물칸에 자원을 가지고 있는 경우 제작 시도
        TryCraft();
    }
    
    private void CacheComponents()
    {
        _outline = GetComponent<Outline>();
    }

    /// <summary>
    /// 화물칸의 자원을 확인하고 제작 조건이 되면 제작 프로세스 시작
    /// </summary>
    public void TryCraft()
    {
        // 1. 이미 제작 중이면 진행 안 함
        if (isCrafting)
        {
            return;
        }

        // 2. 제작칸 보관함이 가득 차 있으면 진행 안 함
        if (currentCraftCount >= maxRailStorage)
        {
            return;
        }

        // 3. 연결된 화물칸이 없으면 진행 안 함
        if (targetCargoCart == null)
        {
            return;
        }

        // 4. 자원 부족 시 진행 안 함 (목재 1개, 철 1개 필요)
        if (targetCargoCart.CurrentWoodCount < 1 || targetCargoCart.CurrentIronCount < 1)
        {
            return;
        }

        // 조건을 만족하면 화물칸 자원 소모 후 제작 코루틴 시작
        Debug.Log("[TestCraftCart] 자원 소모 성공! 레일 제작을 시작합니다...");
        targetCargoCart.ConsumeResources(1, 1);
        StartCoroutine(CraftRoutine());
    }

    /// <summary>
    /// 제작 타이머 대기 후 레일 1개를 누적
    /// </summary>
    private IEnumerator CraftRoutine()
    {
        // 제작 상태 시작 및 비주얼 갱신
        isCrafting = true;
        UpdateCraftVisual();

        yield return new WaitForSeconds(craftTime);

        // 레일 1개 생산 및 수량 제한 처리
        currentCraftCount++;
        Debug.Log($"[TestCraftCart] 레일 1개 제작 완료! (현재 제작칸 레일 수: {currentCraftCount}/{maxRailStorage})");

        // 레일 최대 소지수 이상이면 제작 중단
        if (currentCraftCount > maxRailStorage)
        {
            currentCraftCount = maxRailStorage;
        }

        isCrafting = false;
        UpdateCraftVisual();

        // 제작 완료 후 남은 자원 및 공간이 있다면 연쇄 제작 시도
        TryCraft();
    }

    /// <summary>
    /// 버튼 상호작용으로 완성된 레일을 플레이어 손 데이터로 전달합니다.
    /// </summary>
    /// <param name="inPlayerHand">플레이어가 손에 들고 있는 아이템 데이터</param>
    /// <returns>상호작용 후 플레이어 손에 전달/누적될 데이터</returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        // 1. 제작칸에 완성된 레일이 없으면 그대로 반환
        if (currentCraftCount <= 0)
        {
            Debug.Log("[TestCraftCart] 제작칸에 완성된 레일이 없습니다.");
            return inPlayerHand;
        }

        TestHandData handData = inPlayerHand as TestHandData;

        // 2. 전달할 공간(플레이어 손의 여유 스택) 계산
        int remainSpace = 0;

        // 플레이어가 이미 레일을 들고 있는 경우 (스택 누적)
        if (handData != null && handData.BlockType == BlockType.Rail)
        {
            remainSpace = handData.MaxCount - handData.Count;
        }
        // 플레이어가 빈손인 경우
        else if (handData == null)
        {
            // 빈손이면 최대 소지량(3개 가정)만큼 전달 공간 확보
            remainSpace = maxRailStorage;
        }
        // 레일이 아닌 다른 자원을 들고 있는 경우 전달 불가
        else
        {
            Debug.Log($"[TestCraftCart] 레일이 아닌 자원({handData.BlockType})을 들고 있어 전달할 수 없습니다.");
            return inPlayerHand;
        }

        // 손에 공간이 없으면 그대로 반환
        if (remainSpace <= 0)
        {
            Debug.Log("[TestCraftCart] 플레이어 손에 여유 공간이 없습니다.");
            return inPlayerHand;
        }

        // 3. 실제 전달할 수량 계산
        int amountToGive = 0;

        if (currentCraftCount >= remainSpace)
        {
            amountToGive = remainSpace;
        }
        else
        {
            amountToGive = currentCraftCount;
        }

        // 제작칸의 레일 보유분 차감
        currentCraftCount -= amountToGive;

        // 4. 플레이어에게 넘겨줄 데이터 구성
        if (handData != null)
        {
            // 기존 들고 있던 레일에 수량 추가
            handData.AddCount(amountToGive);
        }
        else
        {
            // 빈손이었던 경우 새로 레일 데이터 생성해서 부여
            handData = new TestHandData(BlockType.Rail, amountToGive);
        }

        Debug.Log($"[TestCraftCart] 레일 {amountToGive}개 전달 완료! (제작칸 남은 레일: {currentCraftCount})");

        // 레일을 꺼내어 제작 공간이 생겼으므로 추가 제작 시도
        TryCraft();

        return handData;
    }

    /// <summary>
    /// 제작 진행 여부 및 보관량에 따라 제작칸 비주얼을 변경
    /// </summary>
    private void UpdateCraftVisual()
    {
        if (currentCraftCount > 0)
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