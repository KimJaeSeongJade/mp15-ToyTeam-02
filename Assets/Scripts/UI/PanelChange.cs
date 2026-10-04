using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Panel을 끄고 켜는 스크립트
/// </summary>
public class PanelChange : InteractUI
{
    [SerializeField] private TitleSceneManager _titleSceneManager;    
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _NextPanel;

    private void Start()
    {
        _titleSceneManager = GameObject.Find("TitleSceneManager").GetComponent<TitleSceneManager>();
    }

    //현재 패널을 끄고, 다음 패널을 켬. TitleSceneManager에 Undo스택 저장
    protected override void PlayUI()
    {        
        _NextPanel.SetActive(true);
        _currentPanel.SetActive(false);
        _titleSceneManager.StackUI.Push((_currentPanel, _NextPanel));
    }    
}
