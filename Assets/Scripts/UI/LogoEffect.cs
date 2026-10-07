using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LogoEffect : MonoBehaviour
{
    public Image logoImage;
    public CanvasGroup canvasGroup; // 투명도 조절용 (추천)

    void Start()
    {
        // 1. 초기 상태 설정 (투명하게, 크기는 약간 작게)
        canvasGroup.alpha = 0f;
        logoImage.transform.localScale = Vector3.one * 0.85f;

        // 2. DOTween 효과 적용
        // 1.5초 동안 페이드인
        canvasGroup.DOFade(1f, 1.5f);

        // 1.5초 동안 원래 크기(1.0)로 커지면서 부드럽게 감속(EaseOutCubic)
        logoImage.transform.DOScale(1f, 1.5f).SetEase(Ease.OutCubic);
    }
}
