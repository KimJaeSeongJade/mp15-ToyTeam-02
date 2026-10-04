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

    //--------------------
    private void Awake()
    {
        SetSingleton();
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