using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRGLevelUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _text;
    private int _currentNum;

    private float _easy;
    private float _normal;
    private float _hard;

    private void Awake()
    {
        Init();
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
        _currentNum = 1;
        SetLevel(_currentNum);
        RefreshUI();
    }

    public void ChangePage(int value)
    {
        int next = _currentNum + value;

        if (next < 0 || next >= _text.Length)
        {
            return;
        }

        _currentNum = next;

        SetLevel(_currentNum);
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
            if (i == _currentNum)
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
