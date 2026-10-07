using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LogoEffect : MonoBehaviour
{
    private RectTransform _logoRect => GetComponent<RectTransform>();

    [SerializeField]private float _dropTime;
    [SerializeField] private float _dropPos;

    void Start()
    {
        _logoRect.anchoredPosition = new Vector2(0, _dropPos);

        _logoRect.DOAnchorPos(Vector2.zero, _dropTime)
                .SetEase(Ease.OutBounce);
    }

}
