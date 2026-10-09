using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRGLevelUI : LRUI
{
    //[SerializeField] private GameObject[] _text;
    //private int _currntNum;

    private void Awake()
    {
        Init();
    }

    // 부모 Start 안받게 하기위한 선언
    private void Start()
    {       
    }

    private void Init()
    {
        SetLevel((int)GameDifficulty.Normal);
    }

    private void OnEnable()
    {
        _currntNum = 1;
        SetLevel(_currntNum);
        RefreshUI();
    }

    public override void ChangePage(int value)
    {
        int next = _currntNum + value;

        if (next < 0 || next >= _text.Length)
        {
            return;
        }

        _currntNum = next;

        SetLevel(_currntNum);
        RefreshUI();
    }

    private void SetLevel(int num)
    {
        GameManager.Instance.SetGameDifficulty((GameDifficulty)num);
    }

    private void RefreshUI()
    {
        for (int i = 0; i < _text.Length; i++)
        {
            if (i == _currntNum)
            {
                _text[i].SetActive(true);
            }
            else
            {
                _text[i].SetActive(false);
            }
        }
    }
}
