using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRGModeSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _text;
    private int _currentNum;

    private void OnEnable()
    {
        _currentNum = 1;
        SetMode(_currentNum);
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

        SetMode(_currentNum);
        Debug.Log(GameManager.Instance.GMode);
        RefreshUI();
    }

    private void SetMode(int num)
    {
        switch (num)
        {
            case 0:
                GameManager.Instance.SetGameMode(GameMode.Infinite);
                break;
            case 1:
                GameManager.Instance.SetGameMode(GameMode.Quick);
                break;
            case 2:
                GameManager.Instance.SetGameMode(GameMode.Test);
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
