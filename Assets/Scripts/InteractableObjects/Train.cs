using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Train : MonoBehaviour, IInteractable
{
    [SerializeField] protected Train headCart; // 앞쪽 열차칸
    [SerializeField] protected Train tailCart; // 뒤쪽 열차칸
    
    [SerializeField] protected float followDistance = 1.0f; // 앞쪽 열차칸 사이의 간격
    [SerializeField] protected float moveSpeed = 3.0f;      // 이동속도
    
    /// <summary>
    /// 자신의 게임 오브젝트
    /// </summary>
    public GameObject GameObject => gameObject;
    /// <summary>
    /// 자신의 블록 종류
    /// </summary>
    public BlockType BlockType => BlockType.None;
  
    /// <summary>
    /// 뒤쪽 열차칸
    /// </summary>
    public Train HeadCart => headCart;
    /// <summary>
    /// 뒤쪽 열차칸
    /// </summary>
    public Train TailCart => tailCart;

    /// <summary>
    /// 열차 생성 시 앞 열차칸과 연결해 주는 함수
    /// </summary>
    /// <param name="fowardCart"> 내 앞에 위치할 열차칸</param>
    public virtual void Initialize(Train fowardCart)
    {
        headCart = fowardCart;
        
        if (headCart != null)
        {
            headCart.tailCart = this;
        }
    }

    protected virtual void Update()
    {
        // 매 프레임 앞차를 따라 이동
        FollowHeadCart();
    }

    /// <summary>
    /// 앞 열차칸과의 거리를 유지한채 따라가며 움직이는 로직
    /// </summary>
    protected virtual void FollowHeadCart()
    {
        // 따라갈 대상
        Transform followTarget;
        
        if (headCart != null)
        {
            followTarget = headCart.transform;
        }
        // 따라갈 열차칸이 없는 경우
        else
        {
            // 엔진칸의 위치를 따라감
            followTarget = GetEngineCartTransform();
        }
        // 따라갈 열차칸이 없으면 예외처리
        if (followTarget == null) return;

        // followTarget과의 거리를 계산하기 위한 기준 간격 변수 선언
        float distance = Vector3.Distance(transform.position, followTarget.position);

        // 정해진 간격(followDistance)이 기준보다 멀어지면 따라감
        if (distance > followDistance)
        {
            // 열차칸을 따라갈 때 y축은 자신의 높이를 기준으로 삼기
            Vector3 targetPosition = followTarget.position;
            targetPosition.y = transform.position.y;
            // 앞 열차칸을 바라보도록 설정
            transform.LookAt(targetPosition);

            // 앞 대상을 향해 조금씩 이동
            transform.position = Vector3.MoveTowards(transform.position, 
                followTarget.position, moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 엔진칸의 위치를 불러오는 함수
    /// </summary>
    protected virtual Transform GetEngineCartTransform()
    {
        // TODO: 엔진칸 스크립트에서 Transform 가져오기
        return null;
    }

    // IInteractable 인터페이스 구현

    /// <summary>
    /// 플레이어가 자동으로 상호작용 하는 경우
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public virtual void AutoInteract(IInteractable interactable) { }

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용 하는 경우
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public abstract IInteractable ButtonInteract(IInteractable interactable);

    // 상호작용 가능 여부에 따라 테두리 켜고 끄기
    public abstract void Targeted();
    public abstract void Untargeted();

}
