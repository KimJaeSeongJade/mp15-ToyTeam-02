using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PanelChange : InteractUI
{
    [SerializeField] private TitleSceneManager _titleSceneManager;
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _NextPanel;
    [SerializeField] private UIBase _stackUI => GetComponent<PanelChange>();

    private void Start()
    {
        _titleSceneManager = GameObject.Find("TitleSceneManager").GetComponent<TitleSceneManager>();
    }

    public override void PlayUI()
    {
        _NextPanel.SetActive(true);
        _currentPanel.SetActive(false);
        _titleSceneManager.StackUI.Push(this);
    }

    public void Undo()
    {
        _currentPanel.SetActive(true);
        _NextPanel.SetActive(false);        
    }
}
