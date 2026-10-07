using UnityEngine;

/// <summary>
/// 튜토리얼 중 "레일 제작 실습" 단계에서 제작칸의 레일 생산을 감지하기 위한 트리거
/// </summary>
public class CraftTrigger : MonoBehaviour
{
    private CraftCart craftCart;

    private void Awake()
    {
        craftCart = GetComponent<CraftCart>();
    }

    private void Update()
    {
        // 제작 수레에 레일이 1개 이상 쌓이면 감지
        if (craftCart.CurrentCraftCount >= 1)
        {

            TestManager manager = FindObjectOfType<TestManager>();
            if (manager != null)
            {
                manager.OnRailCrafted();
            }

            // 트리거 감지 후 자신 비활성화
            this.enabled = false;
        }
    }
}