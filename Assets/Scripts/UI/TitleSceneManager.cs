using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
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

    public void OppenSetting()
    {
        _isSettingOpen = true;
        SceneManagerA.Instance.Pause();
        _settingUI.SetActive(true);
    }

    private void CloseSetting()
    {
        _isSettingOpen = false;
        SceneManagerA.Instance.Continue();
        _settingUI.SetActive(false);
    }

    // 게임 종료
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Appleication.Quit();
        #endif
    }

}
