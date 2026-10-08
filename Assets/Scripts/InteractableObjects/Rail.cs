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
    [SerializeField] private GameObject _lineRailPrefabTop;
    [SerializeField] private GameObject _lineRailPrefabMiddle;
    [SerializeField] private GameObject _lineRailPrefabBottom;
    
    /// <summary>
    /// 곡선 레일 모양: 4방향으로 꺽이는 형태 4가지로 구분 
    /// </summary>
    [SerializeField] private GameObject _curveRailPrefab;
    
    /// <summary>
    /// true라면 타일로써의 레일, false라면 아이템으로써의 레일 상태
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
        UpdateStackVisuals();
    }
    
    // 오브젝트 풀에서 꺼내질 때 초기화
    private void OnEnable()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }

        _currentCount = 1;
        ChangeRailShape(RailShape.HorizontalLine);
        UpdateStackVisuals();
        _currentCount = 1;
    }

    public void UpdateStackVisuals()
    {
        switch (Count)
        {
            case 3:
                _lineRailPrefabTop.SetActive(true);
                _lineRailPrefabMiddle.SetActive(true);
                _lineRailPrefabBottom.SetActive(true);
                _lineRailPrefabTop.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabMiddle.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabBottom.transform.eulerAngles = new Vector3(0, 45, 0);
                break;
            case 2:
                _lineRailPrefabTop.SetActive(false);
                _lineRailPrefabMiddle.SetActive(true);
                _lineRailPrefabBottom.SetActive(true);
                _lineRailPrefabTop.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabMiddle.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabBottom.transform.eulerAngles = new Vector3(0, 45, 0);
                break;
            case 1:
                _lineRailPrefabTop.SetActive(false);
                _lineRailPrefabMiddle.SetActive(false);
                _lineRailPrefabBottom.SetActive(true);
                _lineRailPrefabTop.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabMiddle.transform.eulerAngles = new Vector3(0, 45, 0);
                _lineRailPrefabBottom.transform.eulerAngles = new Vector3(0, 45, 0);
                break;
            case 0:
                break;
        }
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
        // 자기 자신 감지 또는 레일의 상태가 Railway 타일 상태라면 합쳐지지 않기
        if (IsRailway ||
            inPlayerHand == null ||
            inPlayerHand.GameObject == gameObject ||
            inPlayerHand.BlockType != BlockType.Rail) return;

        // 플레이어가 접근한 오브젝트가 Rail인 경우
        Rail inPlayerRail = inPlayerHand as Rail;

        int totalCount = _currentCount + inPlayerRail.Count;
            
        if(totalCount > _maxStack)
        {
            inPlayerRail._currentCount = _maxStack;
            _currentCount = totalCount - _maxStack;
        }
        else
        {
            inPlayerRail._currentCount = totalCount;
            _currentCount = 0;
            Map.Instance.SetHoldable(transform.position.WorldToCoord(), null);
            ReturnToPool();
        }

        Rail rail = inPlayerHand as Rail;
        rail.UpdateStackVisuals();
        UpdateStackVisuals();
    }

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용 당하는 경우
    /// (1. 레일 타일 설치 / 2. 레일 타일 회수 
    /// </summary>
    /// <param name="inPlayerHand"> 플레이어가 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        if (inPlayerHand != null && inPlayerHand.GameObject == gameObject)
            return inPlayerHand;

        if (!IsRailway)
        {
            return this;
        }
        else
        {
            if (inPlayerHand != null && inPlayerHand.BlockType != BlockType.Rail)
                return inPlayerHand;

            // 플레이어가 손에 이미 레일을 최대치 들고 있는 경우 손에 든 오브젝트 반환
            if (inPlayerHand != null && inPlayerHand.BlockType == BlockType.Rail)
            {
                Rail inPlayerRail = inPlayerHand as Rail;
                if (inPlayerRail.Count == _maxStack)
                {
                    return inPlayerHand;
                }
            }
            if (RailManager.Instance.Rails.Last.Value == this)
            {
                IsRailway = false;
                RailManager.Instance.RemoveLastRailway();

                // 플레이어가 뽑아 타일에서 제거되어 레일아이템 상태가 된 자신을 반환
                return this;
            }
            return inPlayerHand;
        }
    }

    /// <summary>
    /// 레일을 배치하여 현재 스택을 하나 줄임
    /// </summary>
    public void ReduceStack()
    {
        _currentCount--;

        if (_currentCount == 0)
        {
            ReturnToPool();
        }
        UpdateStackVisuals();
    }
    
    /// <summary>
    /// 버튼을 눌러 레일을 바닥에 설치하면 설치된 레일 객체의 상태를 타일로 변경
    /// </summary>
    public void SetupRailway()
    {
        // 맵에 설치되었으므로 레일타일로 상태변경
        IsRailway = true;
        _currentCount = 1;

        // 설치한 레일이므로 로드된 청크에 저장
        Map.Instance.AddLoadedPoolable(this);
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

        gameObject.SetActive(false);
        ObjectPool.Instance.Return(this);
    }
    
    /// <summary>
    /// 레일의 설치와 해제시 RailManager가 호출함
    /// "1.새롭게 설치되는 레일 타일" 및 "2.레일이 회수되어 새롭게 마지막 타일로 지정된 타일"에 해당
    /// RailManager의 계산 결과에 따라 레일의 프리팹(직선 or 곡선 모양) 및 회전 각도를 변경
    /// </summary>
    /// <param name="railShape">설정할 레일 모양 Enum</param>

    /// <summary>
    /// 연결된 railway에 맞도록 레일의 모양 변경
    /// </summary>
    /// <param name="railShape"></param>
    public void ChangeRailShape(RailShape railShape)
    {
        
        switch (railShape)
        {
            // 1. 직선 레일의 모양 구현하기
            
            // 1.1 세로 방향의 직선 레일 모양 만들기
            case RailShape.VerticalLine:
                _lineRailPrefabBottom.SetActive(true);
                _curveRailPrefab.SetActive(false);
                _lineRailPrefabBottom.transform.rotation = Quaternion.identity;
                break;
            // 2.1 가로 방향의 직선 레일 모양 만들기
            case RailShape.HorizontalLine:
                _lineRailPrefabBottom.SetActive(true);
                _curveRailPrefab.SetActive(false);
                _lineRailPrefabBottom.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            
            // 2. 곡선 레일의 모양 구현하기
            // 벡터의 `x축 -90도`는 레일 타일을 바닥에 눕히는 기준 각도입니다.
            // 벡터의 `y축` 값을 90도씩 회전 시켜 4개의 방향을 만듭니다.
            
            // 2.1 ⬇️➡️ '└' 모양 곡선 레일
            case RailShape.DownToRightCurve:
                _lineRailPrefabBottom.SetActive(false);
                _curveRailPrefab.SetActive(true);
                // 곡선 레일 프리팹 기준 초기 회전값
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 0, 0);
                break;
            
            // 2.2 ⬆️➡️ '┌' 모양 곡선 레일
            case RailShape.UpToRightCurve:
                _lineRailPrefabBottom.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 90, 0);
                break;
            
            // 2.3 ⬆️⬅️ '┐' 모양 곡선 레일
            case RailShape.UpToLeftCurve:
                _lineRailPrefabBottom.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 180, 0);
                break;
            
            // 2.4 ⬇️⬅️ '┘' 모양 곡선 레일
            case RailShape.DownToLeftCurve:
                _lineRailPrefabBottom.SetActive(false);
                _curveRailPrefab.SetActive(true);
                _curveRailPrefab.transform.eulerAngles = new Vector3(-90, 270, 0);
                break;
        }
    }
}
