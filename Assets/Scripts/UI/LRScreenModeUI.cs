using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 창모드 관리 LR UI
/// </summary>
public class LRScreenModeUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _text;
    private int _currentNum;

    private void OnEnable()
    {
        _currentNum = SceneManagerA.Instance.ScreenModeNum;
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

        SceneManagerA.Instance.ChangeScreenMode(_currentNum);

        RefreshUI();
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