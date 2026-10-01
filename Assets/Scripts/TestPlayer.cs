using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
   [SerializeField] private Rail railOnMap;
   [SerializeField] private Rail railInHand;

   
   private void Update()
   {
      if (railOnMap == null) return;
      
      if (Input.GetKeyDown(KeyCode.A))
      {
         Debug.Log("[상호작용 전]");
         PrintState();
         Debug.Log(" TestPlayer: AutoInteract 적용 시도(바닥의 레일아이템을 흡수 ");
         railOnMap.AutoInteract(railInHand);
         Debug.Log("[상호작용 후]");
         PrintState();
      }
         
   }

   private void PrintState()
   {
      Debug.Log($"맵에 위치한 레일의 타입: {railOnMap.BlockType} / 바닥 레일 갯수 : {railOnMap.Count}");
      Debug.Log($"손에 든 레일의 타입: {railInHand.BlockType} / 손에든 레일 갯수 : {railInHand.Count}");
      
      if (railOnMap.BlockType == BlockType.RailWay)
      {
         Debug.Log($"맵에 위치한 레일의 타입이 {railOnMap.BlockType}라서, 중첩할 수 없습니다");
      }
      
   }
   
}
