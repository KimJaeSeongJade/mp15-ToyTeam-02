using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 관리 Singleton Manager
/// </summary>
public class SceneManagerA : MonoBehaviour
{
    private static SceneManagerA _instance;
    public static SceneManagerA Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = _instance = FindObjectOfType<SceneManagerA>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    private int _screenModeNum;
    private int _ResolutionNum;

    //TitleScene
    public bool _isSelectNow;

    //--------------------
    private void Awake()
    {
        SetSingleton();
        _screenModeNum = 1;
        _ResolutionNum = 1;
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(3);
    }

    // 게임 정지
    public void Pause()
    {
        Time.timeScale = 0f;
        GameManager.Instance.IsPause = true;
    }
    // 게임 Pause해제
    public void Continue()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPause = false;
    }

    // 게임 창모드 변경
    public void ChangeScreenMode(int value)
    {
        _screenModeNum += value;
        switch (value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }

    // 게임 해상도 변경
    public void SetResolution(int value)
    {
        _ResolutionNum += value;
        switch (value)
        {
            case 0:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;
        }
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent <SceneManagerA>();
        DontDestroyOnLoad(gameObject);
    }
}