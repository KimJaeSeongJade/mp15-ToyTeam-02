using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TitleScene관리 매니저
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    private KeyCode _esc => KeyCode.Escape;
    [SerializeField] private GameObject _undoUI;
    [SerializeField] private GameObject _quitUI;
    [SerializeField] private GameObject _settingUI;      

    private bool _isSettingOpen;

    public Stack<(GameObject, GameObject)> StackUI = new ();
    
    private void Update()
    {  
        if(StackUI.Count == 0)
        {
            _quitUI.SetActive(true);
            _undoUI.SetActive(false);
        }
        else
        {
            _quitUI.SetActive(false);
            _undoUI.SetActive(true);
        }

        if(_isSettingOpen && Input.GetKeyDown(_esc))
        {
            CloseSetting();
        }
    }

    // UI뒤로가기
    public void Undo()
    {
        if(StackUI.Count < 1)
        {
            return;
        }

        else
        {
            (GameObject, GameObject) undoUI = StackUI.Pop();
            undoUI.Item1.SetActive(true);
            undoUI.Item2.SetActive(false);
        }
    }
    // Setting창 열기
    public void OppenSetting()
    {
        SceneManagerA.Instance._isSelectNow = false;
        _isSettingOpen = true;
        SceneManagerA.Instance.Pause();
        _settingUI.SetActive(true);
        _settingUI.gameObject.GetComponent<MenuController>()._isSelect = true;
        _settingUI.gameObject.GetComponent<MenuController>()._isSettingUI = false;
    }
    // Setting창 닫기
    private void CloseSetting()
    {
        if (!SceneManagerA.Instance._isSelectNow)
        {
            _isSettingOpen = false;
            SceneManagerA.Instance.Continue();
            _settingUI.SetActive(false);
        }
    }
        
    public void ChangeScreenMode(int value)
    {
        SceneManagerA.Instance.ChangeScreenMode(value);
    }

    public void SetResolution(int value)
    {
        SceneManagerA.Instance.SetResolution(value);
    }


    public void PlayerAdd()
    {

    }

    public void PlayerDel()
    {

    }

    // 게임 종료
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
