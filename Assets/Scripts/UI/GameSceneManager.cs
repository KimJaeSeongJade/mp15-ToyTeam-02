using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [SerializeField] private LocomotiveCart _locomotiveCart;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _trainSpeedUI;
    [SerializeField] private LoadSceneConnector _loadSceneConnector;

    private KeyCode _pauseKey => KeyCode.Escape;

    public event Action OnGameEnd;

    public bool IsGameEnd { get; private set; }
    public bool IsGameWin { get; private set; }

    private void Awake()
    {
        SetSingleton();
        Init();
    }
    private void Start()
    {
        BindGameEvents();       
    }
    private void OnDestroy() => UnbindGameEvents();

    private void Update()
    {
        
        if(!SceneManagerA.Instance._isPause && Input.GetKeyDown(_pauseKey))
        {
            StartCoroutine(OpenPause());            
        }
        SetTrainSpeed();
        
    }

    private void SetTrainSpeed()
    {
        float speed = GameManager.Instance.TrainSpeed;
        _trainSpeedUI.GetComponent<TextNumChanger>().ChangeNum(speed);
    }

    // ClosePause와 키가 동시에 눌려 코루틴으로 사용
    private IEnumerator OpenPause()
    {
        yield return null;        
        SceneManagerA.Instance._isPause = true;
        SceneManagerA.Instance.Pause();
        _pauseMenu.SetActive(true);
        _pauseMenu.GetComponent<PMenuController>()._isSelect = true;
    }

    private IEnumerator ClosePause()
    {
        if (!SceneManagerA.Instance._isSelectNow)
        {
            SceneManagerA.Instance._isPause = false;
            SceneManagerA.Instance.Continue();
            _pauseMenu.SetActive(false);
            yield return null;
        }
    }
        
    public void ClosePause2()
    {
        if (!SceneManagerA.Instance._isSelectNow)
        {
            SceneManagerA.Instance._isPause = false;
            SceneManagerA.Instance.Continue();
            _pauseMenu.SetActive(false);           
        }
    }

    // 다른 씬 로드하면 파괴되는 싱글톤
    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }    

    private void Init()
    {
        _pauseMenu.SetActive(false);
    }

    private void GameOver()
    {
        IsGameWin = false;
        GameManager.Instance.SetIsGameWin(IsGameWin);
        _loadSceneConnector.LoadEndScene();
    }

    private void GameClear()
    {
        IsGameWin = true;
        
        // 튜토리얼 클리어시 튜토리얼 매니저 호출
        
        // 테스트용 테스트매니저 클래스로 연결 - 테스트 끝나면 삭제하고 아래 코드 주석 해제
        TestManager tutorialManager = FindObjectOfType<TestManager>();
        // TutorialManager tutorialManager FindObjectOfType<TestManager>();
        
        if (tutorialManager != null)
        {
            tutorialManager.OnGameClear();
        }
        GameManager.Instance.SetIsGameWin(IsGameWin);
        _loadSceneConnector.LoadEndScene();
    }

    private void BindGameEvents()
    {
        _locomotiveCart.OnTrainArrived += GameOver;
        RailManager.Instance.OnRailwayConnected += GameClear;
    }

    private void UnbindGameEvents()
    {
        _locomotiveCart.OnTrainArrived -= GameOver;
        RailManager.Instance.OnRailwayConnected -= GameClear;
    }
}
