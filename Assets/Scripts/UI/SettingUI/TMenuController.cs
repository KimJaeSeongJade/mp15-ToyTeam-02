using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TMenuController : MonoBehaviour
{
    private KeyCode _upKey => KeyCode.W;
    private KeyCode _downKey => KeyCode.S;
    private KeyCode _inKey => KeyCode.D;
    private KeyCode _outKey => KeyCode.Escape;

    private UIConnector uiconnector;

    [SerializeField] private GameObject _fistPanelList;
    [SerializeField] private GameObject _secondPanelList;

    [SerializeField] private TMenuController _otherTMenu;

    private List<GameObject> _panel = new();
    private int _currenNum;

    private bool _isSecond;

    //지금 조작할 UI인지
    public bool _isSelect;

    //--------------------

    private void OnEnable()
    {
        _panel = _fistPanelList.GetComponent<PanelListConnector>().PannelList;

        for (int i = 0; i < _panel.Count; i++)
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
        if (!_isSecond)
        {
            _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(true);
        }
    }

    private void OnDisable()
    {
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
        if (!_isSecond)
        {
            _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(false);
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
    }

    private void Up()
    {
        if (_currenNum > 0)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            if (!_isSecond)
            {
                _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(false);
            }
            _currenNum--;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
            if (!_isSecond)
            {
                _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(true);
            }
        }
    }

    private void Down()
    {
        if (_currenNum < _panel.Count - 1)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            if (!_isSecond)
            {
                _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(false);
            }
            _currenNum++;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
            if (!_isSecond)
            {
                _secondPanelList.GetComponent<PanelListConnector>().PannelList[_currenNum].SetActive(true);
            }
        }
    }

    private void In()
    {       
        _isSelect = false;
        _otherTMenu.gameObject.GetComponent<PanelListConnector>().PannelList[_currenNum].gameObject.GetComponent<TMenuController>()._isSelect = true;
        _otherTMenu.gameObject.GetComponent<PanelListConnector>().PannelList[_currenNum].gameObject.GetComponent<TMenuController>()._isSecond = true;
    }

    private void Out()
    {
        SceneManagerA.Instance._isSelectNow = false;
        _isSelect = false;
        _otherTMenu._isSelect = true;
    }
}