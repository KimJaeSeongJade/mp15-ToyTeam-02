using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour, IInteractable, IStackable, IPoolable
{
    [SerializeField] private int currentCount = 1;   // 현재 중첩 갯수
    [SerializeField] private int maxStack = 3;       // 최대 중첩 갯수
    
    // true라면 타일로써의 레일, flase라면 아이템으로써의 레일 상태
    [SerializeField] private bool isRailWay = false;
    private Outline _outline;
    
    /// <summary>
    /// 자신의 게임 오브젝트
    /// </summary>
    public GameObject GameObject => gameObject;

    /// <summary>
    /// 자신의 블록 종류
    /// </summary>
    public BlockType BlockType
    {
        get
        {   
            // 레일의 상태가 레일 타일일 경우
            if (isRailWay == true)
            {
                return BlockType.RailWay;
            }
            // 레일의 상태가 레일 아이템인 경우
            else
            {
                return BlockType.Rail;
            }
        }
    }
    
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
    
    // 오브젝트 풀에서 꺼내질 때 초기화
    private void OnEnable()
    {
     
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
    /// 플레이어가 raycast로 자동 상호작용 (레일 아이템 합쳐지기)
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable inPlayerHand)
    {
        // 레일의 상태가 RailWay 타일 상태라면 합쳐지지 않기
        if (isRailWay) return;
        
        // 플레이어가 접근한 오브젝트가 Rail 이면서, 아이템으로써의 레일인 경우
        if (inPlayerHand is Rail RailOnHand && RailOnHand.BlockType == BlockType.Rail) 
        {
            // 자기자신과 상호작용하지 않게 예외처리
            if (RailOnHand == this) return;
            
            // 중첩가능한 카운트를 계산하고 더 중첩할 수 없다면 예외처리
            int leftCount = RailOnHand.maxStack - RailOnHand.currentCount;
            if (leftCount <= 0) return;

            int getAmount;
            
            // 중첩가능한 수보다 레일아이템의 갯수가 더 많은 경우
            if ( leftCount < currentCount )
            {
                // 더 중첩할수 있는 갯수만 가져온다
                getAmount =  leftCount;
            }
            // 레일아이템의 갯수를 전부 중첩 가능한 경우
            else
            {
                // 모두 가져와 중첩한다
                getAmount = currentCount;
            }
            
            // 대상의 개수를 가져와 중첩시키고 현재 레일아이템 중첩 수 증가
            RailOnHand.currentCount += getAmount;
            currentCount -= getAmount;
            
            // 합쳐져서 남은 수량이 0이된 레일 아이템을 오브젝트 풀로 반환
            if (currentCount <= 0)
            {
                ReturnToPool(this);
            }
        }
    }

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용
    /// </summary>
    /// <param name="inPlayerHand"> 플레이어가 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        // 1. 플레이어가 버튼을 눌러 손에든 레일아이템을 설치하는 경우
        if (inPlayerHand != null && inPlayerHand.BlockType == BlockType.Rail)
        {
            
            // 기존의 맵위에 레일타일이 설치되지 않았을경우에만 레일 타일 설치
            if (!isRailWay)
            {
                SetupRailWay();
            
                // 손에든 레일 타일은 그대로 유지
                return inPlayerHand; 
            }
            
        }
        
        // 2. 아무것도 들지 않은채로 버튼을 눌러 레일타일을 회수하는 경우 
        if (inPlayerHand == null)
        {
           
            if (isRailWay)
            {
               
                RemoveRailWay();
            }
            // 플레이어가 뽑아 타일에서 제거되어 레일아이템 상태가 된 자신을 반환
            return this;
        }
        
        // 3. 조건에 맞지 않는경우는 플레이어의 손에 든 상태 유지
        return inPlayerHand;
    }
    
    /// <summary>
    /// 레일타일을 맵에 배치
    /// </summary>
    public void SetupRailWay()
    {
        // 맵에 설치되었으므로 레일타일로 상태변경
        isRailWay = true;
        currentCount = 1;
        
        /* Todo: 설치될때 레일매니저 클래스를 통해 레일 모양 변경
        SetRailLink()
        */
    }

    /// <summary>
    /// 레일타일을 맵에서 제거하 
    /// </summary>
    public void RemoveRailWay()
    {
      
        // 타일에서 제거되었으므로 레일 아이템으로 상태변경
        isRailWay = false;
        
        ReturnToPool(this);
        
        /* 설치될때 레일매니저 클래스를 통해 레일 모양 변경해야 하므로
        SetRailLink()
        */
    }

    /// <summary>
    /// 자신의 외곽선 표시 활성화
    /// </summary>
    public void Targeted()
    {
        // 플레이어 감지범위내에 레일이 있을때 테두리 켜기
        _outline.enabled = true;
    }

    /// <summary>
    /// 자신의 외곽선 표시 비활성화
    /// </summary>
    public void Untargeted()
    {
        // 플레이어 감지범위를 벋어나면 테두리 끄기
        _outline.enabled = false;
    }

    /// <summary>
    /// 자기 자신을 오브젝트 풀로 반환
    /// </summary>
    /// <param name="poolable"> 자기 자신의 IPoolable </param>
    public void ReturnToPool(IPoolable poolable)
    {
        
        if (_outline != null)
        {
            _outline.enabled = false;
        }
        
        isRailWay = false;
        currentCount = 1;
        
        // 오트젝트풀 클래스로 자기 자신을 반환해야 함
        // ObjectPool.Instance.Return(this.gameObject);
        
        gameObject.SetActive(false);
    }
    
}
