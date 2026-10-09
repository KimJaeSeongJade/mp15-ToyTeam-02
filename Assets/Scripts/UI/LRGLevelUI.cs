using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRGLevelUI : LRUI
{
    //[SerializeField] private GameObject[] _text;
    //private int _currntNum;

    private float _easy;
    private float _normal;
    private float _hard;

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
        _easy = 0.05f;
        _normal = 0.075f;
        _hard = 0.1f;
        SetLevel(1);
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
        switch (num)
        {
            case 0:
                GameManager.Instance.SetTrainSpeed(_easy);
                break;
            case 1:
                GameManager.Instance.SetTrainSpeed(_normal);
                break;
            case 2:
                GameManager.Instance.SetTrainSpeed(_hard);
                break;
            default:
                break;
        }
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
