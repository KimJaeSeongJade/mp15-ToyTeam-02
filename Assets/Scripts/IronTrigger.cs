using UnityEngine;

/// <summary>
/// 튜토리얼 중 "레일 제작 실습" 단계에서 제작칸의 레일 생산을 감지하기 위한 트리거
/// </summary>
public class IronTrigger : MonoBehaviour
{
    private bool _isInitialLoad = true;

    private void OnEnable()
    {
        // Debug.Log("Iron trigger enabled");
        if (_isInitialLoad) _isInitialLoad = false;
        else if (TutorialManager.Instance != null && !_isInitialLoad)
        {
            TutorialManager.Instance.OnRockMined();
        }
    }
}