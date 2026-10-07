using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Text 깜박이는 효과
/// </summary>
public class UIBlink : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _text;

    [SerializeField]private LoopType _loopType;

    private void Start()
    {
        _text.alpha = 0f;
        StartCoroutine(BlinkEffet());
    }

    private IEnumerator BlinkEffet()
    {        
        yield return new WaitForSeconds(1f);
        _text.DOFade(1f, 1).SetLoops(-1, _loopType);
    }
}
