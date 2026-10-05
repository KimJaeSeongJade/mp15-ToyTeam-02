using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DummyPlayer : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("상호작용 키")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("[더미 상태] 플레이어 소지 자원 설정")]
    [SerializeField] private BlockType holdingBlockType = BlockType.Wood;
    [SerializeField] private int handCount = 1;
    [SerializeField] private int playerMaxCount = 3; // 플레이어가 최대 소지 가능한 수량

    private IInteractable currentTarget;
    private TestCraftCart targetCraftCart; // 제작칸 전용 참조
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        TestInteraction();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0f, v).normalized;
        _rb.velocity = new Vector3(direction.x * moveSpeed, _rb.velocity.y, direction.z * moveSpeed);

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void TestInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // 1. 타겟이 제작칸(TestCraftCart)인 경우
            if (targetCraftCart != null)
            {
                // 1.1) 플레이어 손에 아이템이 들어있는 경우
                if (handCount > 0 && holdingBlockType != BlockType.None)
                {
                    // 플레이어가 손에 들고 있는 아이템 더미 데이터 생성
                    TestHandData handData = new TestHandData(holdingBlockType, handCount, playerMaxCount);

                    // 제작칸에 손 데이터를 넘겨주고 상호작용 결과 받기
                    IInteractable result = targetCraftCart.ButtonInteract(handData);
                    TestHandData resultData = result as TestHandData;

                    // 제작칸에서 처리 후 반환된 아이템 정보로 플레이어 손 데이터 갱신
                    if (resultData != null)
                    {
                        holdingBlockType = resultData.BlockType;
                        handCount = resultData.Count;
                        Debug.Log($"[DummyPlayer] 플레이어 아이템 보유: {holdingBlockType} ({handCount}/{playerMaxCount}개)");
                    }
                }
                // 1.2) 플레이어가 빈손인 경우
                else
                {
                    // 제작칸에 빈손(null)을 전달하여 레일을 받아옴
                    IInteractable result = targetCraftCart.ButtonInteract(null);
                    TestHandData resultData = result as TestHandData;

                    // 제작칸에서 전달받은 레일 데이터가 있는 경우
                    if (resultData != null)
                    {
                        holdingBlockType = resultData.BlockType;
                        handCount = resultData.Count;
                        Debug.Log($"[DummyPlayer] 레일 획득 성공: {holdingBlockType} ({handCount}/{playerMaxCount}개)");
                    }
                    else
                    {
                        Debug.Log("[DummyPlayer] 제작칸에 완성된 레일이 없습니다.");
                    }
                }
            }
            
            // 2. 타겟이 화물칸(TestCargoCart 등)인 경우
            else if (currentTarget != null)
            {
                // 2.1) 플레이어가 손에 재료를 들고 있는 경우 -> 화물칸에 자원 투입(Push)
                if (handCount > 0 && holdingBlockType != BlockType.None)
                {
                    // 플레이어가 손에 들고 있는 아이템 더미 데이터 생성
                    TestHandData handItem = new TestHandData(holdingBlockType, handCount, playerMaxCount);
                    
                    // 화물칸에 손 데이터를 넘겨주고 상호작용 결과 받기
                    IInteractable result = currentTarget.ButtonInteract(handItem);
                    TestHandData remainItem = result as TestHandData;
                    
                    // 화물칸 용량이 가득 차서 손에 자원이 일부 남은 경우
                    if (remainItem != null)
                    {
                        holdingBlockType = remainItem.BlockType;
                        handCount = remainItem.Count;
                        Debug.Log($"[DummyPlayer] 화물칸에 공간이 부족해 재료가 남았습니다: {holdingBlockType} ({handCount}개)");
                    }
                    
                    // 자원이 화물칸에 모두 전부 투입된 경우
                    else
                    {
                        handCount = 0;
                        holdingBlockType = BlockType.None;
                        Debug.Log("[DummyPlayer] 화물칸에 모든 재료를 투입하여 빈손이 되었습니다.");
                    }
                }
                
                // 2.2) 플레이어 손이 비어있는 경우 -> 화물칸에서 자원 꺼내기 (Give)]
                else
                {
                    // 버튼 상호작용을 null로 호출하여 전달하여 화물칸 자원 꺼내기 요청
                    IInteractable result = currentTarget.ButtonInteract(null);
                    // 화물칸에서 건네준 재료르 더미 데이터로 변환
                    
                    TestHandData takenItem = result as TestHandData;

                    // 화물칸에서 재료를 꺼내온 경우
                    if (takenItem != null)
                    {
                        // 화물칸에서 꺼낸 재료를 플레이어 손으로 전달
                        holdingBlockType = takenItem.BlockType;
                        handCount = takenItem.Count;
                        Debug.Log($"[DummyPlayer] 화물칸에서 재료를 가져왔습니다: {holdingBlockType} ({handCount}개)");
                    }
                    // 화물칸에 재료가 없는 경우
                    else
                    {
                        Debug.Log("[DummyPlayer] 화물칸에 보관된 자원이 없습니다.");
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null)
        {
            currentTarget = interactable;
            
            currentTarget.Targeted(); 
            
            Debug.Log($"[DummyPlayer] {other.name} 감지");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        
        if (interactable != null && interactable == currentTarget)
        {
            currentTarget.Untargeted(); 
            
            Debug.Log($"[DummyPlayer] {other.name} 감지 범위 벗어남");
            currentTarget = null;
        }
    }
}

/// <summary>
/// 테스트 환경 전용 아이템 전달용 데이터 클래스
/// </summary>
public class TestHandData : IInteractable
{
    public BlockType BlockType { get; set; }
    public int Count { get; set; }
    public int MaxCount { get; set; }

    public TestHandData(BlockType type, int count, int maxCount = 3)
    {
        BlockType = type;
        Count = count;
        MaxCount = maxCount;
    }

    public void AddCount(int amount)
    {
        Count += amount;
    }

    public GameObject GameObject => null;
    public void AutoInteract(IInteractable interactable) { }
    public IInteractable ButtonInteract(IInteractable interactable) => null;
    public void Targeted() { }
    public void Untargeted() { }
}