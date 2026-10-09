using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreEffect : MonoBehaviour
{
    [SerializeField] private float _delayTime;
    [SerializeField] private float _fadeTime;
    private CanvasGroup _canvasG;

    private void Awake()
    {
        _canvasG = GetComponent<CanvasGroup>();
        _canvasG.alpha = 0;
    }


    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(_delayTime);
        _canvasG.DOFade(1, _fadeTime);
    }
}
