using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 좌우로 텍스트를 넘기는 UI
/// </summary>
public class LRUI : MonoBehaviour
{
    //화면에 보일 Text 오브젝트 배열
    [SerializeField]private GameObject[] _text;
    // 현재 페이지 숫자
    private int _currntNum;
    // 이전 페이지 숫자
    private int _prevNum;

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        CashComponent();
    }
    private void Init()
    {
        _currntNum = 1;
        _prevNum = 1;
    }    

    private void CashComponent()
    {
        _text[0].gameObject.SetActive(false);
        _text[1].gameObject.SetActive(true);
        _text[2].gameObject.SetActive(false);
    }
    // 유니티 이벤트에 등록시킬 페이지 바꾸는 매서드
    public void ChangePage(int num)
    {
        _currntNum += num;

        if(_currntNum <= 0)
        {
            _currntNum = 0;
        }
        if (_currntNum >= _text.Length)
        {
            _currntNum = _prevNum;
            return;
        }

        _text[_prevNum].SetActive(false);
        _text[_currntNum].SetActive(true);
        _prevNum = _currntNum;
    }
}
