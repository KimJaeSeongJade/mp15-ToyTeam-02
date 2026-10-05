using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설정 조작 가이드 이미지
/// </summary>
public class ShowControlImage : MonoBehaviour
{
    [SerializeField] private GameObject _selectImage;
    [SerializeField] private GameObject _selectImage2;

    private MenuController _menucontroller;

    private void Start()
    {        
        _menucontroller = GetComponentInParent<MenuController>();
        _selectImage.SetActive(false);
        _selectImage2.SetActive(true);
    }

    private void Update()
    {
        if(_menucontroller._isSelect == true)
        {
            _selectImage.SetActive(true);
            _selectImage2.SetActive(false);
        }
        else 
        {
            _selectImage.SetActive(false);
            _selectImage2.SetActive(true);
        }
    }

}
