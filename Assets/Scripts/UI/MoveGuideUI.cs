using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveGuideUI : MonoBehaviour
{
    private CanvasGroup _canvas => GetComponent<CanvasGroup>();
    private bool _isFade;

    private void Awake()
    {
        _isFade = false;
    }

    private void Update()
    {
        if(!_isFade &&(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.W)))
        {
            StartCoroutine(CloseUI());
        }
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private IEnumerator CloseUI()
    {
        _isFade = true;
        yield return new WaitForSeconds(1.5f);
        _canvas.DOFade(0, 1.5f);
    }    
}
