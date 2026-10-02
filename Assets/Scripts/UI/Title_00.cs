using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_00 : InteractUI
{
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _NextPanel;

    public override void PlayUI()
    {
        _NextPanel.SetActive(true);
        _currentPanel.SetActive(false);        
    }
}
