using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    // 마지막 레일 판단을 쉽게하기위해 링크드 리스트형태로 구현
    public LinkedList<Rail> Rails = new LinkedList<Rail>();
    

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


    public Rail GetLastRailway()
    {
        return Rails.Last.Value;
    }

    public void TryRailWayPlace(Rail targetRail, Vector2Int coord)
    {
        Vector2Int lastRailCoord = Rails.Last.Value.transform.position.WorldToCoord();
        
        if (lastRailCoord.x == coord.x && Mathf.Abs(lastRailCoord.y - coord.y) == 1 )
        {
            AddRailWay(targetRail);
        }

    }


    private void AddRailWay(Rail targetRail)
    {
        // Todo: 맵의 해당 타일을 RailWay로 교체
        LinkedListNode<Rail> prevRailNode = Rails.Last;
        Rails.AddLast(targetRail);
        
        ChangeRailWayShape(prevRailNode);
        ChangeRailWayShape(Rails.Last);
    }

    private void ChangeRailWayShape(LinkedListNode<Rail> targetNode)
    {
        Rail targetRail = targetNode.Value;
        Vector2Int currentRailCoord = targetNode.Value.transform.position.WorldToCoord();
        Vector2Int prevRailCoord = targetNode.Previous.Value.transform.position.WorldToCoord();

        if (targetNode.Next == null) 
        {
            if (prevRailCoord.x == currentRailCoord.x)
            {
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
            else if (prevRailCoord.y == currentRailCoord.y)
            {
                targetRail.ChangeRailShape(RailShape.VerticalLine);
            }
        }
        else
        {
          
            Vector2Int nextRailCoord = targetNode.Next.Value.transform.position.WorldToCoord();
            
            if (prevRailCoord.x == nextRailCoord.x)
            {
                targetRail.ChangeRailShape(RailShape.HorizontalLine);
            }
            
            else if (prevRailCoord.y == nextRailCoord.y)
            {
                targetRail.ChangeRailShape(RailShape.VerticalLine);
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
    
    
    





}
