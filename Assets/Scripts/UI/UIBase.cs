using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI기본기능 추상 클래스
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    // 각 UI별 기능 작동
    public abstract void PlayUI();
    // UI활성화
    private void OnUI()
    {
        gameObject.SetActive(true);
    }
    // UI비활성화
    private void OffUI()
    {
        gameObject.SetActive(false);
    }
}
