using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour, IInteractable, IStackable, IPoolable
{
    /// <summary>
    /// 레일 객체의 현재까지 중첩된 소지 갯수 
    /// </summary>
    [SerializeField] private int _currentCount = 1; 
    
    /// <summary>
    /// 레일 객체의 최대 소지 갯수 
    /// </summary>
    [SerializeField] private int _maxStack = 3;
    
    /// <summary>
    /// 직선 레일 모양: 수직, 수평으로 구분 
    /// </summary>
    [SerializeField] private GameObject _lineRailPrefab;
    
    /// <summary>
    /// 곡선 레일 모양: 4방향으로 꺽이는 형태 4가지로 구분 
    /// </summary>
    [SerializeField] private GameObject _curveRailPrefab;   // 
    
    /// <summary>
    /// true라면 타일로써의 레일, flase라면 아이템으로써의 레일 상태
    /// </summary>
    [field: SerializeField] public bool IsRailway { get; private set; }
    
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
    
    private Outline _outline;
    
    // 컴포넌트 초기화
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
    /// (1. 레일 타일 설치 / 2. 레일 타일 회수 
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
                // 맵에 설치되기 위해 플레이어 손에 들린 자신을 반환하여 호출될 준비
                return this; 
            }
            // 손에든 것이 레일 아이템이 아니라면
            else
            {
                // 원래 들고있는 IInteractable 상속 객체를 반환
                return inPlayerHand;
            }
            
        }
        
        // 2. 아무것도 들지 않은채로 버튼을 눌러 레일타일을 회수하는 경우 
        
        // 플레이어가 손에 아무것도 들지 않았는지 확인
        if (inPlayerHand == null)
        {
            // 상호작용을 시도하는 레일의 상태가 레일 타일이 맞는지 확인
            if (IsRailway)
            {
                // 회수하려는 레일 타일이 열차가 통과하지 않은 레일 타일인지 확인
                if (RailManager.Instance.Rails.Last.Value == this)
                {
                    // 맞다면 레일을 제거
                    RemoveRailway();
                    // 제거되어 레일아이템 상태가 된 자신을 반환
                    return this;
                }
            }
        }
        
        // 두 상황 모두 아니면 플레이어의 손에 든 상태 유지
        return inPlayerHand;
    }
    
    /// <summary>
    /// 버튼을 눌러 레일을 바닥에 설치하면 설치된 레일 객체의 상태를 타일로 변경
    /// </summary>
    public void SetupRailway()
    {
        // 맵에 설치되었으므로 레일타일로 상태변경
        IsRailway = true;
        _currentCount = 1;
    }

    /// <summary>
    /// 버튼을 눌러 설치된 레일 타일을 회수하는 경우에 호출되어 레일 제거
    /// </summary>
    public void RemoveRailway()
    {
      
        // 타일에서 제거되었으므로 타일 상태에서 아이템으로 상태 변경
        IsRailway = false;
        
        // RailManager를 통해 새롭게 마지막이된 레일 타일의 모양과 레일의 연결 관계를 재설정
        RailManager.Instance.RemoveLastRailway();

        // 제거되었으므로 해당 타일을 오브젝트풀로 반환
        ReturnToPool();
        
    }

    /// <summary>
    /// 플레이어가 접근하여 상호작용 가능한 경우 테두리 표시
    /// </summary>
    public void Targeted()
    {
        // "레일이 아이템 상태" 또는 "맨 마지막에 설치된 레일 타일"일 때
        if (!IsRailway || this == RailManager.Instance.Rails.Last.Value) 
            // 테두리 적용
            _outline.enabled = true;
    }

    /// <summary>
    /// 플레이어의 타게팅에서 벗어날 경우 테두리 비활성화
    /// </summary>
    public void Untargeted()
    {
        // 테두리 비활성화
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
        
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 레일의 설치와 해제시 RailManager가 호출함
    /// "1.새롭게 설치되는 레일 타일" 및 "2.레일이 회수되어 새롭게 마지막 타일로 지정된 타일"에 해당
    /// RailManager의 계산 결과에 따라 레일의 프리팹(직선 or 곡선 모양) 및 회전 각도를 변경
    /// </summary>
    /// <param name="railShape">설정할 레일 모양 Enum</param>

    public void ChangeRailShape(RailShape railShape)
    {
        
        switch (railShape)
        {
            // 1. 직선 레일의 모양 구현하기
            
            // 1.1 세로 방향의 직선 레일 모양 만들기
            case RailShape.VerticalLine:
                // 직선 레일 모양 적용
                _lineRailPrefab.SetActive(true);
                // 곡선 레일 모양 비적용
                _curveRailPrefab.SetActive(false);
                // 회전은 기본값(0도, 회전 없음) 적용 -> 세로 형태로 설정
                _lineRailPrefab.transform.rotation = Quaternion.identity;
                break;
            // 2.1 가로 방향의 직선 레일 모양 만들기
            case RailShape.HorizontalLine:
                // 직선 레일 모양 적용
                _lineRailPrefab.SetActive(true);
                // 곡선 레일 모양 비적용
                _curveRailPrefab.SetActive(false);
                // 회전은 Y축으로 90도 회전 -> 가로 형태로 설정
                _lineRailPrefab.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            
            // 2. 곡선 레일의 모양 구현하기
            // 벡터의 `x축 -90도`는 레일 타일을 바닥에 눕히는 기준 각도입니다.
            // 벡터의 `y축` 값을 90도씩 회전 시켜 4개의 방향을 만듭니다.
            
            // 2.1 (┌ 방향 곡선]: ⬇️➡️ 
            case RailShape.DownToRightCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                // 곡선 레일 프리팹 기준 초기 회전값
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 0, 0);
                break;
            
            // 2.2 (└ 방향 곡선): ⬆️➡️
            case RailShape.UpToRightCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 90, 0);
                break;
            
            // 2.3 (┘ 방향 곡선):  ⬆️⬅️
            case RailShape.UpToLeftCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 180, 0);
                break;
            
            // 2.4 (┐ 방향 곡선): ⬇️⬅️
            case RailShape.DownToLeftCurve:
                _lineRailPrefab.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 270, 0);
                break;
        }
    }
}
