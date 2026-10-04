using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DummyPlayer : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("상호작용 키")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("플레이어 소지 자원 (인스펙터 조절)")]
    [SerializeField] private BlockType holdingBlockType = BlockType.Wood;
    [SerializeField] private int handCount = 1;

    private IInteractable currentTarget;
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
        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            // 플레이어 손에 자원이 있는 경우 -> 화물칸에 자원 투입 (Push)
            if (handCount > 0 && holdingBlockType != BlockType.None)
            {
                Debug.Log($"[재료 넣기 시도] 손에 든 자원: {holdingBlockType} ({handCount}개)");

                // 1. 손에 든 자원 정보를 전달할 데이터 객체 생성
                TestHandData handItem = new TestHandData(holdingBlockType, handCount);

                // 2. 화물칸에 전달하고 결과(남은 자원) 받아오기
                IInteractable result = currentTarget.ButtonInteract(handItem);

                // 3. 결과 확인: 화물칸 용량이 모자라 손에 자원이 남은 경우 (result != null)
                TestHandData remainItem = result as TestHandData;
                if (remainItem != null)
                {
                    holdingBlockType = remainItem.BlockType;
                    handCount = remainItem.Count;
                    Debug.Log($"[재료 넣기] 손에 자원이 남았습니다: {holdingBlockType} ({handCount}개)");
                }
                
                // 4. 전부 수납되어서 빈손이 된 경우 (result == null)
                else
                {
                    handCount = 0;
                    Debug.Log("[재료 넣기] 화물칸에 모두 넣었습니다. (handCount = 0)");
                }
            }
           
            // [플레이어 손이 비어있는 경우 -> 화물칸에서 자원 회수 (Give)
            else
            {
                Debug.Log("[재료 꺼내기 시도] 빈손 상태에서 재료를 꺼냅니다.");

                // 1. 빈손(null)을 전달하여 화물칸 자원을 가져옴
                IInteractable result = currentTarget.ButtonInteract(null);

                // 2. 화물칸에서 꺼내온 자원이 있는 경우
                TestHandData takenItem = result as TestHandData;
                if (takenItem != null)
                {
                    holdingBlockType = takenItem.BlockType;
                    handCount = takenItem.Count;
                    Debug.Log($"[재료 꺼내기 성공] 화물칸에서 자원을 가져왔습니다: {holdingBlockType} ({handCount}개)");
                }
                else
                {
                    Debug.Log("[꺼내기 실패] 화물칸이 비어있습니다.");
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
            currentTarget.Targeted(); // 테두리 ON
            Debug.Log($"[감지 범위 진입] {other.name} - 테두리 ON");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable == currentTarget)
        {
            currentTarget.Untargeted(); // 테두리 OFF
            Debug.Log($"[감지 범위 이탈] {other.name} - 테두리 OFF");
            currentTarget = null;
        }
    }
}

/// <summary>
/// TestCargoCart의 PushResource에서 테스트용으로 인식시키기 위한 더미 데이터
/// </summary>
public class TestHandData : IInteractable
{
    public BlockType BlockType { get; set; }
    public int Count { get; set; }

    public TestHandData(BlockType type, int count)
    {
        BlockType = type;
        Count = count;
    }

    public void AddCount(int amount)
    {
        Count += amount;
    }

    // IInteractable 인터페이스 구현
    public GameObject GameObject => null;
    public void AutoInteract(IInteractable interactable) { }
    public IInteractable ButtonInteract(IInteractable interactable) => null;
    public void Targeted() { }
    public void Untargeted() { }
}