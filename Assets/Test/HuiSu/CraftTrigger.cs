using UnityEngine;

[RequireComponent(typeof(CraftCart))]
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