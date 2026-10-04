using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    /// <summary>
    /// 열차가 움직일 경로를 생성/관리하는 매니저 클래스
    /// </summary>
    [SerializeField] private SplineManager _splineManager;

    /// <summary>
    /// 설치된 레일의 [이전 레일(Prev)]과 [다음 레일(Next)]에 빠르게 접근하기 위해 링크드 리스트 적용
    /// '이전 레일'과 '다음 레일'을 탐색하여 "레일의 설치와 회수"에 따른,
    /// 레일의 연결 모양(직선/곡선 및 회전) 계산에 활용
    /// </summary>
    public LinkedList<Rail> Rails = new LinkedList<Rail>();
    
    /// <summary>
    /// RailManager를 싱글톤으로 설정
    /// Player, Map, Rail, RailManager, 각 열차칸 등에서 레일 관리를 바로 접근 할 수 있음
    /// </summary>
    public static RailManager Instance { get; private set; }

    private void Awake()
    {
        SetSingleton();
    }

    //싱글톤 설정
    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    /// <summary>
    /// 현재 배치된 레일 타일 중 가장 마지막에 위치한 레일 객체를 반환
    /// 레일 설치 및 회수 가능한지 판단, 열차가 레일 끝에 도달했는지 등 판단에 쓰임
    /// </summary>
    public Rail GetLastRailway()
    {
        return Rails.Last.Value;
    }

    /// <summary>
    /// 맵 좌표(coord)에 레일이 설치 가능한지 판단하고 가능하다면 레일 배치
    /// </summary>
    /// <param name="coord">설치하려는 타일의 그리드 좌표</param>
    /// <returns>설치 성공하면 true, 실패 하면 false</returns>
    public bool TryRailwayPlace(Vector2Int coord)
    {
        // 상황 1. 설치하려는 위치에 이미 다른 오브젝트가 존재한다면 설치할 수 없음
        if (Map.Instance.GetHoldable(coord) != null)
        {
            return false;
        }
        
        // 상황 2. 레일이 담긴 링크드 리스트의 숫자가 0일때
        //    -> 최초의 레일을 설치하는 경우는 설치 가능
        if (Rails.Count == 0)
        {
            AddRailway(coord);
            return true;
        }
        
        // 마지막 레일의 위치정보를 좌표값으로 변환하여 담은 변수 선언
        Vector2Int lastRailCoord = Rails.Last.Value.transform.position.WorldToCoord();
        
        // 상황 3. 통상적으로 레일을 설치하는 경우
        // 레일 설치를 위한 좌표계 위치 판단 조건
        //   -> 좌표계 기준 상 / 하 / 좌 / 우 1칸 거리로 인접해있는 경우에만 레일 설치 가능
        //   -> 수직(상 또는 하), 수평(좌 또는 우) 2가지 경우로 구분
        
        // 3.1. 레일이 수직 방향으로 인접한 경우 = x좌표 값이 같고, y좌표값의 차이가 1인 경우 또는
        // 3.2. 레일이 수평 방향으로 인접한 경우 = y좌표 값이 같고, x좌표값의 차이가 1인 경우
        if (lastRailCoord.x == coord.x && Mathf.Abs(lastRailCoord.y - coord.y) == 1 ||
            lastRailCoord.y == coord.y && Mathf.Abs(lastRailCoord.x - coord.x) == 1)
        {
            AddRailway(coord);
            return true;
        }
        // 설치가 가능하다는 조건들을 통과하지 못했다면 설치 실패 반환
        return false;
    }
    
    
    // 좌표값을 전달 받아 실제 맵상에 레일 설치
    private void AddRailway(Vector2Int coord)
    {
        // 1단계 공용 오브젝트풀에서 대상을 레일블록으로 지정
        IPoolable poolable = ObjectPool.Instance.Take(BlockType.Rail);
        Rail newRail = poolable as Rail;
        
        // 2. 그리드 기준 좌표값을 실제 3D 월드 기준의 위치로 바꾸고 새로운 레일객체를 활성화
        newRail.GameObject.transform.position = coord.CoordToWorld();
        newRail.GameObject.SetActive(true);
        
        // 3. Map 싱글톤 접근: 플레이어가 들 수 있는 오브젝트를 맵 좌표에 저장
        //    SplineManager 클래스 접근: 열차가 지나가는 railway(spline knot)를 생성
        Map.Instance.SetHoldable(coord, newRail);
        _splineManager.AddSplineKnot(coord);

        // 4. 설치된 레일 객체의 상태를 타일로 변경
        newRail.SetupRailway();

        // 5. 새로 설치된 레일 객체를 링크드 리스트에 추가
        Rails.AddLast(newRail);

        // 6. 신규 레일 설치에 따라 인접한 레일의 모양 및 회전 모습 갱신
        UpdateRailwayShape();
    }
    
    // 모양을 바꿀 대상 레일 선정 : 새로 설치된 레일 및 기존에 가장 마지막이였던 레일
    private void UpdateRailwayShape()
    {
        // (신규 레일 설치 전) 기존 마지막 레일의 직전 노드를 가져와 변수에 대입
        LinkedListNode<Rail> prevRailNode = Rails.Last.Previous;

        // 연결된 이전 레일이 없는 경우 = 맵에 깔린 레일이 오직 1개 뿐인 경우
        // -> 모양 계산 중단 
        if (prevRailNode == null)
        {
            return;
        }
            
        // 이전 레일이 첫 번째 레일(First)가 아닌 경우 = 통상적인 중간 레일인 경우
        // 이전 레일 모양 재계산
        if (prevRailNode != Rails.First) ChangeRailwayShape(prevRailNode);
        
        // 모든 경우에도 새로 설치된 레일의 모양은 바뀜
        ChangeRailwayShape(Rails.Last);
    }

    /// <summary>
    /// 대상 레일 노드의 이전(Prev), 현재(Current), 다음(Next) 격자 좌표를 비교하여,
    /// 직선 레일 또는 4방향 곡선(= 곡선 레일)을 판별해 레일의 외형을 변경
    /// " 신규 설치할 레일"의 모양과
    /// " 신규 설치된 레일의 '이전 레일' "의 모양이 바뀌는 두가지 상황 발생 
    /// </summary>
    /// <param name="targetNode">모양을 계산할 대상 레일의 노드</param>
    private void ChangeRailwayShape(LinkedListNode<Rail> targetNode)
    {
        // 타겟 노드의 값을, Rail 클래스로 만들어 변수에 대입
        Rail targetRail = targetNode.Value;
        // 타겟팅된 현재 Rail객체의 3D월드 위치를 좌표값으로 만들어 변수에 대입
        Vector2Int currentRailCoord = targetNode.Value.transform.position.WorldToCoord();
        // 이전 Rail 객체의 3D월드 위치를 좌표값으로 만들어 변수에 대입
        Vector2Int prevRailCoord = targetNode.Previous.Value.transform.position.WorldToCoord();

        // 상황 1
        // 다음 레일을 가지고 있지 않는 경우 = '신규 설치'할 레일의 모양을 바꾸는 경우
        //  -> 다음 레일이 없으므로, 이전 레일 방향으로 직선 레일 모양으로 연결
        if (targetNode.Next == null) 
        {
            // 1.1 이전 레일과 현재 레일의 x 좌표가 같은경우 = ↕️ 수직(상/하)으로 연결된 경우
            if (prevRailCoord.x == currentRailCoord.x)
            {
                // 직선 레일 모양을 세로 형태로 배치
                targetRail.ChangeRailShape(RailShape.VerticalLine);
            }
            // 1.2 레일들의 y 좌표가 같은경우 = ↔️ 수평(좌/우)으로 연결된 경우
            else if (prevRailCoord.y == currentRailCoord.y)
            {
                // 직선 레일 모양을 가로 형태로 배치
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
        }
        
        // 상황 2
        // 1번 상황아 아닌 경우 = 신규 설치 레일 직전의 '중간 레일'을 바꾸는 경우
        else
        {
            // 다름 Rail 객체의 3D월드 위치를 좌표값으로 만들어 변수에 대입
            Vector2Int nextRailCoord = targetNode.Next.Value.transform.position.WorldToCoord();
            
            // 1. 이전 레일과 다음 레일의 x 좌표가 동일 = 수직으로 관통 직진 ↕️
            if (prevRailCoord.x == nextRailCoord.x)
            {
                // 직선 레일 모양을 세로 형태로 배치
                targetRail.ChangeRailShape(RailShape.VerticalLine);
            }
            // 2. 이전 레일과 다음 레일의 y 좌표가 동일 = 수평으로 관통 직진 ↔️
            else if (prevRailCoord.y == nextRailCoord.y)
            {
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
            
            // 3. 이전 레일과 현재 레일의 x 좌표가 동 일하고 
            else if (prevRailCoord.x == currentRailCoord.x)
            {
                if (prevRailCoord.y < currentRailCoord.y)
                {
                    if (currentRailCoord.x < nextRailCoord.x )
                    {
                        targetRail.ChangeRailShape(RailShape.UpToRightCurve);
                    }
                    else
                    {
                        targetRail.ChangeRailShape(RailShape.UpToLeftCurve);
                    }
                }
                
                else
                {
                    if (currentRailCoord.x < nextRailCoord.x)
                    {
                        targetRail.ChangeRailShape(RailShape.DownToRightCurve);
                    }
                    else
                    {
                        targetRail.ChangeRailShape(RailShape.DownToLeftCurve);
                    }
                    
                }
                
            }
            
            
            else if (prevRailCoord.y == currentRailCoord.y)
            {
                if (prevRailCoord.x < currentRailCoord.x)
                {
                    if (currentRailCoord.y < nextRailCoord.y)
                    {
                        targetRail.ChangeRailShape(RailShape.DownToLeftCurve);
                    }
                    else
                    {
                        targetRail.ChangeRailShape(RailShape.UpToLeftCurve);
                    }
                }

                else
                {
                    if (currentRailCoord.y < nextRailCoord.y)
                    {
                        targetRail.ChangeRailShape(RailShape.DownToRightCurve);
                    }
                    
                    else
                    {
                        targetRail.ChangeRailShape(RailShape.UpToRightCurve);
                    }
                }
            }
        }    
    }

    /// <summary>
    /// 마지막에 설치한 Railway 제거
    /// </summary>
    public void RemoveLastRailway()
    {
        if (Rails.Count == 0) return;

        Map.Instance.SetHoldable(Rails.Last.Value.GameObject.transform.position.WorldToCoord(), null);
        _splineManager.RemoveLastSplineKnot();
        Rails.RemoveLast();

        UpdateRailwayShape();
    }
}
