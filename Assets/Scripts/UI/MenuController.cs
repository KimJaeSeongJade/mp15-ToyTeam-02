using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private KeyCode _upKey => KeyCode.UpArrow;
    private KeyCode _downKey => KeyCode.DownArrow;
    private KeyCode _inKey => KeyCode.RightArrow;
    private KeyCode _outKey => KeyCode.LeftArrow;

    private UIConnector uiconnector;

    [SerializeField] private List<GameObject> _panel = new();
    private int _currenNum;

    // 지금 조작할 UI인지
    public bool _isSelect;
    
    //--------------------

    private void OnEnable()
    {
        for(int i = 0; i<_panel.Count;i++)
        {
            _panel[i].transform.Find("Image").GetComponent<Image>().enabled = false;
            if (_panel[i].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(false);
            }
        }
        _currenNum = 0;
        _isSelect = true;
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
        if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
        {
            uiconnector.ChildUI.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
        if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
        {
            uiconnector.ChildUI.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_inKey))
        {
            In();
        }
        if (_isSelect)
        {
            if (Input.GetKeyDown(_upKey))
            {
                Up();
            }
            if (Input.GetKeyDown(_downKey))
            {
                Down();
            }
        }
    }

    private void Up()
    {
        if (_currenNum > 0)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            if(_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(false);
            }
            _currenNum--;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
            if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(true);
            }
        }
    }

    private void Down()
    {
        if (_currenNum < _panel.Count - 1)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(false);
            }
            _currenNum++;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
            if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(true);
            }
        }
    }

    private void In()
    {        
        if(uiconnector.ChildUI.gameObject.TryGetComponent<MenuController>(out MenuController t))
        {
            Debug.Log("찾음");
        }
        else 
        {
            Debug.Log("못찾음");
        }
        //uiconnector.ChildUI.gameObject.GetComponentInChildren<MenuController>()._isSelect = true;
        _isSelect = false;
    }

    private void Out()
    {

    }
}
