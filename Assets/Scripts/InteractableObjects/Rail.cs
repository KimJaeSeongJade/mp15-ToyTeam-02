using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour, IInteractable, IStackable, IPoolable
{
    [SerializeField] private int _currentCount = 1;   // 현재 중첩 갯수
    [SerializeField] private int _maxStack = 3;       // 최대 중첩 갯수
    [SerializeField] private GameObject _lineRailPrefab;
    [SerializeField] private GameObject _curveRailPrefab;


    /// <summary>
    /// true라면 타일로써의 레일, flase라면 아이템으로써의 레일 상태
    /// </summary>
    [field: SerializeField] public bool IsRailway { get; private set; }

    private Outline _outline;
    
    /// <summary>
    /// 자신의 게임 오브젝트
    /// </summary>
    public GameObject GameObject => gameObject;

    /// <summary>
    /// 자신의 블록 종류
    /// </summary>
    public BlockType BlockType => BlockType.Rail;
    
    /// <summary>
    /// 쌓인 개수
    /// </summary>
    public int Count => _currentCount;
    
    /// <summary>
    /// 최대로 소지 가능한 개수
    /// </summary>
    public int MaxStack => _maxStack;

    
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

        ChangeRailShape(RailShape.HorizontalLine);
    }

    private void CacheComponents()
    {
        _outline = GetComponent<Outline>();
    }

    /// <summary>
    /// 플레이어가 범위 내로 다가왔을 때 실행되는 자동으로 상호작용 당하는 경우
    /// (손에든 레일 아이템에 바닥에 놓인 레일 아이템 수량 합치기)
    /// </summary>
    /// <param name="inPlayerHand"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable inPlayerHand)
    {
        // 플레이어가 접근한 레일의 상태가 Railway 상태(레일 타일)라면 합쳐지지 않도록 확인
        if (IsRailway) return;
        
        // 플레이에가 손에든 IInteractable 구현 객체가 레일 블록타입인지 확인
        if (inPlayerHand.BlockType == BlockType.Rail)
        {
            // 블록타입이 확인되면 손에든 오브젝트에 Rail 클래스 속성 부여
            Rail railInPlayer = inPlayerHand as Rail;

            // 손에든 레일 아이템과 상호작용 당하는 레일 아이템의 총 갯수 계산
            int totalCount = _currentCount + railInPlayer.Count;
            
            // 두 레일 아이템의 총 갯수가 플레이어가 소지할 수 있는 갯수보다 클경우
            if(totalCount > _maxStack)
            {
                // 플레이어의 손에는 최대 소지수만큼 돌려주고
                railInPlayer._currentCount = _maxStack;
                // 그 나머지를 상호작용 당하는 레일 아이템의 갯수로 지정한다 
                _currentCount = totalCount - _maxStack;

            }
            // 두 레일 아이템의 갯수를 합쳐도 플레이어의 최대 소지 샛수보다 적을 경우
            else
            {
                // 플레이어가 소지한 레일아이템의 갯수는 두 레일 아이템의 갯수를 합친 것
                railInPlayer._currentCount = totalCount;
                // 플레이어쪽으로 레일아이템이 합쳐졌으므로 상호작용 당한 레일아이템은 오브젝트풀로 반환
                _currentCount = 0;
                ReturnToPool();
            }
            
        }
    }

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용 당하는 경우
    /// (1. 레일 타일 설치 / 2. 레일 타일 회수 / 3. 제작차에서 완성된 레일 아이템 들기)
    /// </summary>
    /// <param name="inPlayerHand"> 플레이어가 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        
        // 1. 플레이어가 버튼을 눌러 손에든 레일아이템을 설치하는 경우
        
        // 플레이어가 레일 아이템을 들었는지 확인
        if (inPlayerHand != null)
        {
            // 플레이어가 손에든 레일의 상태가 '아이템'인지 '타일' 인지 확인
            
            // 손에든 것이 레일 아이템 이라면
            if (!IsRailway)
            {
                // 맵에 설치되기 위해 플레이어 손에들린 자신을 반환
                return this; 
            }
            else
            {
                return inPlayerHand;
            }
            
        }
        
        // 2. 아무것도 들지 않은채로 버튼을 눌러 레일타일을 회수하는 경우 
        if (inPlayerHand == null)
        {
            if (IsRailway)
            {
                // todo: 열차가 지나간 Railway는 상호작용 불가능하게
                if (RailManager.Instance.Rails.Last.Value == this)
                {
                    RemoveRailway();
                    // 플레이어가 뽑아 타일에서 제거되어 레일아이템 상태가 된 자신을 반환
                    return this;
                }
                
            }
        }
        
        // 3. 조건에 맞지 않는경우는 플레이어의 손에 든 상태 유지
        return inPlayerHand;
    }
    
    /// <summary>
    /// 레일타일을 맵에 배치
    /// </summary>
    public void SetupRailway()
    {
        // 맵에 설치되었으므로 레일타일로 상태변경
        IsRailway = true;
        _currentCount = 1;
        
        /* Todo: 설치될때 레일매니저 클래스를 통해 레일 모양 변경
        SetRailLink()
        */
    }

    /// <summary>
    /// 레일타일을 맵에서 제거하 
    /// </summary>
    public void RemoveRailway()
    {
      
        // 타일에서 제거되었으므로 레일 아이템으로 상태변경
        IsRailway = false;
        RailManager.Instance.RemoveLastRailway();

        ReturnToPool();
        
        /* 설치될때 레일매니저 클래스를 통해 레일 모양 변경해야 하므로
        SetRailLink()
        */
        // 
    }

    /// <summary>
    /// 자신의 외곽선 표시 활성화
    /// </summary>
    public void Targeted()
    {
        // 플레이어 감지범위내에 레일이 있을때 테두리 켜기
        if (!IsRailway || this == RailManager.Instance.Rails.Last.Value)
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
    public void ReturnToPool()
    {
        
        if (_outline != null)
        {
            _outline.enabled = false;
        }
        
        IsRailway = false;
        _currentCount = 1;
        
        // 오트젝트풀 클래스로 자기 자신을 반환해야 함
        // ObjectPool.Instance.Return(this);
        
        gameObject.SetActive(false);
    }

    public void ChangeRailShape(RailShape railShape)
    {
        switch (railShape)
        {
            case RailShape.VerticalLine:
                _lineRailPrefab.SetActive(true);
                _curveRailPrefab.SetActive(false);
                _lineRailPrefab.transform.rotation = Quaternion.identity;
                break;
            case RailShape.HorizontalLine:
                _lineRailPrefab.SetActive(true);
                _curveRailPrefab.SetActive(false);
                _lineRailPrefab.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            
            case RailShape.DownToRightCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 0, 0);
                break;
            case RailShape.UpToRightCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 90, 0);
                break;
            case RailShape.UpToLeftCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 180, 0);
                break;
            case RailShape.DownToLeftCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 270, 0);
                break;
        }
    }
}
