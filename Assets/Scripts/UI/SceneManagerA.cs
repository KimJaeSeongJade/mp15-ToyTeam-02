using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

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
                _instance = FindObjectOfType<SceneManagerA>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    [SerializeField] private AudioMixer _audioMixer;

    private int _screenModeNum;
    private int _resolutionNum;

    private float _bgmVolume;
    private float _sfxVolume;

    public int ScreenModeNum => _screenModeNum;
    public int ResolutionNum => _resolutionNum;

    public float BgmVolume => _bgmVolume;
    public float SfxVolume => _sfxVolume;

    // 게임이 멈춰있는지
    public bool _isPause;

    // TitleScene, GameScene Menu,PMenuController
    public bool _isSelectNow;

    //--------------------
    private void Awake()
    {
        SetSingleton();
        Init();
    }

    private void Init()
    {
        _isPause = false;
        _screenModeNum = 1;
        _resolutionNum = 1;
        _bgmVolume = 100f;
        _sfxVolume = 100f;
    }       

    public void LoadTitleScene()
    {
        FadeEffect.Instance.FadeOut();
        ResetScene();
        SceneManager.LoadScene(1);
        FadeEffect.Instance.FadeIn();
    }    

    public void LoadTutorialScene()
    {
        FadeEffect.Instance.FadeOut();
        ResetScene();
        SceneManager.LoadScene(2);
        FadeEffect.Instance.FadeIn();
    }
    public void LoadEndScene()
    {
        FadeEffect.Instance.FadeOut();
        ResetScene();
        //GameManager.Instance.ResetPlayTime();
        SceneManager.LoadScene(3);
        FadeEffect.Instance.FadeIn();
    }

    public void LoadGameScene()
    {
        FadeEffect.Instance.FadeOut();
        ResetScene();
        GameManager.Instance.ResetPlayTime();
        if (GameManager.Instance.GMode == GameMode.Tutorial)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(4);
        }
        FadeEffect.Instance.FadeIn();
    }

    private void ResetScene()
    {
        _isPause = false;
        _isSelectNow = false;

        Continue();
    }

    // 게임 정지
    public void Pause()
    {
        Time.timeScale = 0f;
        GameManager.Instance.IsPause(true);
        SetSfxVolume(_sfxVolume);
    }
    // 게임 Pause해제
    public void Continue()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPause(false);
        SetSfxVolume(_sfxVolume);
    }

    // 게임 창모드 변경
    public void ChangeScreenMode(int value)
    {
        _screenModeNum = Mathf.Clamp(value, 0, 2);
        Debug.Log($"ResolutionNum: {_screenModeNum}");
        switch (_screenModeNum)
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
            default:
                break;
        }
    }

    // 게임 해상도 변경
    public void SetResolution(int value)
    {        
        _resolutionNum = Mathf.Clamp(value, 0, 2);
        Debug.Log($"ResolutionNum: {_resolutionNum}");
        switch (_resolutionNum)
        {
            case 0:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                Debug.Log($"해상도 변경: ({Screen.width}x{Screen.height})");
                break;
            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                Debug.Log($"해상도 변경: ({Screen.width}x{Screen.height})");
                break;
            case 2:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                Debug.Log($"해상도 변경: ({Screen.width}x{Screen.height})");
                break;
            default:
                break;
        }
    }

    // 오디오 크기 저장
    public void SetBgmVolume(float value)
    {
        Debug.Log(
       $"[BGM 변경] {value}\n" +
       StackTraceUtility.ExtractStackTrace()
   );

        _bgmVolume = Mathf.Clamp(value, 0f, 100f);

        float volume = _bgmVolume / 100f;

        float dB;

        if (volume > 0f)
        {
            dB = Mathf.Log10(volume) * 20f;
        }
        else
        {
            dB = -80f;
        }

        _audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSfxVolume(float value)
    {
        Debug.Log(
     $"[SFX 변경] {value}\n" +
     StackTraceUtility.ExtractStackTrace()
 );

        _sfxVolume = Mathf.Clamp(value, 0f, 100f);

        float volume = _sfxVolume / 100f;

        float dB;

        if (volume > 0f)
        {
            dB = Mathf.Log10(volume) * 20f;
        }
        else
        {
            dB = -80f;
        }

        if (!_isPause)
            _audioMixer.SetFloat("SFXVolume", dB);
        else _audioMixer.SetFloat("SFXVolume", -80f);
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