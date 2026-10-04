using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public MenuController _prevMenu;

    private KeyCode _upKey => KeyCode.W;
    private KeyCode _downKey => KeyCode.S;
    private KeyCode _inKey => KeyCode.D;
    private KeyCode _outKey => KeyCode.Escape;
    private KeyCode _LeftKey => KeyCode.A;

    private UIConnector uiconnector;

    [SerializeField] private List<GameObject> _panel = new();
    private int _currenNum;

    // 지금 조작할 UI인지
    public bool _isSelect;
    // 제목선택인지 설정조작을 하는 UIPanel인지
    public bool _isSettingUI;
    
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
        _isSelect = false;
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
            if (Input.GetKeyDown(_inKey))
            {
                In();
            }
            if (Input.GetKeyDown(_outKey))
            {
                Out();
            }
        }
        if(_isSettingUI)
        {
            if (Input.GetKeyDown(_inKey))
            {
                SetCurrentUI(1);
            }
            if (Input.GetKeyDown(_LeftKey))
            {
                SetCurrentUI(-1);
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
        if (!_isSettingUI)
        {
            uiconnector.ChildUI.gameObject.GetComponentInChildren<MenuController>()._isSettingUI = true;
            uiconnector.ChildUI.gameObject.GetComponentInChildren<MenuController>()._isSelect = true;
            uiconnector.ChildUI.gameObject.GetComponentInChildren<MenuController>()._prevMenu = gameObject.GetComponent<MenuController>();
            SceneManagerA.Instance._isSelectNow = true;
            _isSelect = false;            
        }
    }

    private void Out()
    {
        SceneManagerA.Instance._isSelectNow = false;
        _isSelect = false;
        _prevMenu._isSelect = true;
    }

    private void SetCurrentUI(int value)
    {
        if (_panel[_currenNum].transform.Find("Slider"))
        {
            _panel[_currenNum].transform.Find("Slider").GetComponent<Slider>().value += (10 * value);
        }
        else if()
        {

        }
    }
}
