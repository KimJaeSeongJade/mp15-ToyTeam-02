using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameScene Pause Menu 컨트롤러
/// </summary>
public class PMenuController : MonoBehaviour
{
    // 설정조작 MenuController에 지금 프리팹 부여
    public PMenuController _prevMenu;
    
    private KeyCode _upKey => KeyCode.W;
    private KeyCode _downKey => KeyCode.S;
    private KeyCode _inKey => KeyCode.Space;
    private KeyCode _outKey => KeyCode.Escape;
    private KeyCode _RightKey => KeyCode.D;
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
        if (!_isSettingUI)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
            if (_panel[_currenNum].TryGetComponent<UIConnector>(out uiconnector))
            {
                uiconnector.ChildUI.gameObject.SetActive(true);
            }
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
                // 코루틴으로 _inKey키 동시 입력 지연
                StartCoroutine(In());
            }
            if (Input.GetKeyDown(_outKey))
            {
                Out();
            }

            if (_isSettingUI)
            {
                if (Input.GetKeyDown(_RightKey))
                {
                    SetCurrentUI(1);
                }
                if (Input.GetKeyDown(_LeftKey))
                {
                    SetCurrentUI(-1);
                }
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

    // 코루틴으로 _inKey키 동시 입력 지연
    // 설정조작UI로 들어감
    private IEnumerator In()
    {
        yield return null;
        if (!_isSettingUI)
        {
            uiconnector.ChildUI.gameObject.GetComponentInChildren<PMenuController>()._isSettingUI = true;
            uiconnector.ChildUI.gameObject.GetComponentInChildren<PMenuController>()._isSelect = true;
            uiconnector.ChildUI.gameObject.GetComponentInChildren<PMenuController>()._prevMenu = gameObject.GetComponent<PMenuController>();
            uiconnector.ChildUI.gameObject.GetComponentInChildren<PMenuController>()._panel[0].transform.Find("Image").GetComponent<Image>().enabled = true;
            
            SceneManagerA.Instance._isSelectNow = true;
            _isSelect = false;
        }
    }

    // 설정조작UI에서 나옴
    private void Out()
    {
        SceneManagerA.Instance._isSelectNow = false;
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
        _isSelect = false;
        _prevMenu._isSelect = true;
    }

    // Silder면 value조절, LRUI면 좌우 넘기기
    private void SetCurrentUI(int value)
    {
        if (_panel[_currenNum].transform.Find("Slider"))
        {
            _panel[_currenNum].transform.Find("Slider").GetComponent<Slider>().value += (10 * value);
        }
        else if(_panel[_currenNum].transform.Find("SetModeLR"))
        {
            switch(value)
            {
                case -1:
                    _panel[_currenNum].transform.Find("SetModeLR").Find("Left").GetComponent<UnityEventInteractUI>().OnUIPressed.Invoke();
                    break;
                case 1:
                    _panel[_currenNum].transform.Find("SetModeLR").Find("Right").GetComponent<UnityEventInteractUI>().OnUIPressed.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}
