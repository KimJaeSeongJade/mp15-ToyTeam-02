using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    public static RailManager Instance { get; private set; }

    [SerializeField] private SplineManager _splineManager;

    public event Action OnRailwayConnected;

    // 마지막 레일 판단을 쉽게하기위해 링크드 리스트형태로 구현
    public LinkedList<Rail> Rails = new LinkedList<Rail>();

    private Vector2Int _endRailCoord = new Vector2Int(-1, -1);
    private List<Vector2Int> _endRails = new();

    private void Awake()
    {
        SetSingleton();
    }

    // 싱글톤 설정
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
    /// 마지막에 설치한 Railway 반환
    /// </summary>
    /// <returns></returns>
    public Rail GetLastRailway()
    {
        return Rails.Last.Value;
    }

    /// <summary>
    /// Railway 설치 할 수 있다면 설치하고 true, 설치할 수 없다면 false
    /// </summary>
    /// <param name="coord"> 맵 좌표 </param>
    /// <returns></returns>
    public bool TryRailwayPlace(Vector2Int coord)
    {
        if (Map.Instance.GetHoldable(coord) != null)
        {
            return false;
        }

        if (Rails.Count == 0)
        {
            AddRailway(coord);
            return true;
        }

        Vector2Int lastRailCoord = Rails.Last.Value.transform.position.WorldToCoord();
        
        if (lastRailCoord.x == coord.x && Mathf.Abs(lastRailCoord.y - coord.y) == 1 ||
            lastRailCoord.y == coord.y && Mathf.Abs(lastRailCoord.x - coord.x) == 1)
        {
            AddRailway(coord);
            return true;
        }
        return false;
    }

    private void AddRailway(Vector2Int coord)
    {
        IPoolable poolable = ObjectPool.Instance.Take(BlockType.Rail);
        Rail newRail = poolable as Rail;

        newRail.GameObject.transform.position = coord.CoordToWorld();
        newRail.GameObject.SetActive(true);

        Map.Instance.SetHoldable(coord, newRail);
        _splineManager.AddSplineKnot(coord);

        newRail.SetupRailway();

        Rails.AddLast(newRail);

        UpdateRailwayShape();

        if (_endRails.Count != 0)
        {
            CheckConnectedWithEndRailway(coord);
        }
    }

    private void UpdateRailwayShape()
    {
        LinkedListNode<Rail> prevRailNode = Rails.Last.Previous;

        if (prevRailNode == null)
        {
            return;
        }

        if (prevRailNode != Rails.First) ChangeRailwayShape(prevRailNode);
        ChangeRailwayShape(Rails.Last);
    }

    private void ChangeRailwayShape(LinkedListNode<Rail> targetNode)
    {
        Rail targetRail = targetNode.Value;
        Vector2Int currentRailCoord = targetNode.Value.transform.position.WorldToCoord();
        Vector2Int prevRailCoord = targetNode.Previous.Value.transform.position.WorldToCoord();

        if (targetNode.Next == null) 
        {
            if (prevRailCoord.x == currentRailCoord.x)
            {
                targetRail.ChangeRailShape(RailShape.VerticalLine);
            }
            else if (prevRailCoord.y == currentRailCoord.y)
            {
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
        }
        else
        {
          
            Vector2Int nextRailCoord = targetNode.Next.Value.transform.position.WorldToCoord();
            if (prevRailCoord.x == nextRailCoord.x)
            {
                targetRail.ChangeRailShape(RailShape.VerticalLine);
            }
            
            else if (prevRailCoord.y == nextRailCoord.y)
            {
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
            
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

        _splineManager.RemoveLastSplineKnot();
        Rails.RemoveLast();

        UpdateRailwayShape();
    }

    /// <summary>
    /// 게임 초기부터 배치되는 마지막 레일 설치
    /// </summary>
    public void PlaceEndRailway(Vector2Int coord)
    { 
        IPoolable poolable = ObjectPool.Instance.Take(BlockType.Rail);
        Rail newRail = poolable as Rail;

        newRail.GameObject.transform.position = coord.CoordToWorld();
        newRail.GameObject.SetActive(true);

        Map.Instance.SetHoldable(coord, newRail);

        newRail.SetupRailway();

        if (_endRailCoord == new Vector2Int(-1, -1))
        {
            _endRailCoord = coord;
        }
        _endRails.Add(coord);

        Debug.Log(coord);
    }

    private void CheckConnectedWithEndRailway(Vector2Int coord)
    {
        if (coord.x == _endRailCoord.x && Mathf.Abs(coord.y - _endRailCoord.y) == 1 ||
            coord.y == _endRailCoord.y && Mathf.Abs(coord.x - _endRailCoord.x) == 1)
        {
            OnRailwayConnected?.Invoke();

            foreach (Vector2Int endRailCoord in _endRails)
            {
                _splineManager.AddSplineKnot(endRailCoord);
            }
        }
    }
}
