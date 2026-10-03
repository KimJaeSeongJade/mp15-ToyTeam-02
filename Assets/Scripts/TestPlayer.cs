using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
   [SerializeField] private Rail railOnMap;
   [SerializeField] private Rail railInHand;
   [SerializeField] private Rail lastRailWayOnMap;
   
   private bool isTargeting;
   
   private void Update()
   {
      if (railOnMap == null) return;
      
      // 자동 상호작용 테스트
      if (Input.GetKeyDown(KeyCode.A))
      {
         AutoInteractTest();
      }
      
      // 상호작용 오브젝트 감지시 테두리 전환
      if (Input.GetKeyDown(KeyCode.Space))
      {
         
         PrefabRotateTest();
         //OutlineTest();
      }
      
      lastRailWayOnMap.Targeted();
      // 버튼을 통한 상호작용 테스트
      if (Input.GetKeyDown(KeyCode.E))
      {
         ButtonInteractTest();
      }
         
   }

   private void PrefabRotateTest()
   {
      railOnMap.ChangeRailShape(RailShape.DownToLeftCurve);
   }
   
   private void AutoInteractTest()
   {
      Debug.Log("[상호작용 전]");
      Debug.Log($"맵에 위치한 레일의 타입: {railOnMap.BlockType} / 바닥 레일 갯수 : {railOnMap.Count}");
      Debug.Log($"손에 든 레일의 타입: {railInHand.BlockType} / 손에든 레일 갯수 : {railInHand.Count}");
      
      railOnMap.AutoInteract(railInHand);
      
      if (railOnMap.IsRailWay)
      {
         Debug.Log($"맵에 위치한 레일이 RailWay라서, 중첩할 수 없습니다");
      }

      Debug.Log(" TestPlayer: AutoInteract 적용 시도 (바닥의 레일아이템을 흡수)");
      Debug.Log($"맵에 위치한 레일의 타입: {railOnMap.BlockType} / 바닥 레일 갯수 : {railOnMap.Count}");
      Debug.Log($"손에 든 레일의 타입: {railInHand.BlockType} / 손에든 레일 갯수 : {railInHand.Count}");
      
   }

   private void OutlineTest()
   {
      isTargeting = !isTargeting;
      if (isTargeting)
      {
         railOnMap.Targeted();
         Debug.Log($"플레이어가 바닥에 놓인 레일 아이템 감지함 / 테두리 활성화");
      }
      else
      {
         railOnMap.Untargeted();
         Debug.Log($"바닥에 놓인 레일 아이템 감지되지 않음 / 테두리 비활성화");
      }
      
   }
   
   private void ButtonInteractTest()
   {
      Debug.Log("[상호작용 전]");
      Debug.Log($"[{lastRailWayOnMap.name}]의 레일 타입: {lastRailWayOnMap.BlockType} ");
      lastRailWayOnMap.ButtonInteract(railInHand);
      Debug.Log("[상호작용 후]");
      Debug.Log($"[{lastRailWayOnMap.name}]의 레일 타입: {lastRailWayOnMap.BlockType} ");
   }
   
   
   
}
