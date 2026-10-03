using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UITypewriter : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMP;
    [SerializeField] Text text;

    private string testT;

    private void Awake()
    {
        testT = "테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트";
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Typing(testT);
        }
    }

    private void Typing(string talk)
    {
        text.text = null;
        text.DOText(talk, 1f);        
    }

    //IEnumerator Typing(string talk)
    //{
    //    text.text = null;
    //    text.DOText(talk, 1f);

    //    다음 대사 딜레이
    //   yield return new WaitForSeconds(1.5f);
    //    NextTalk();
    //}

}
