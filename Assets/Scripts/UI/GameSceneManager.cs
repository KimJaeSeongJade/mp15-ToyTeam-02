using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [SerializeField] private PlayDataManager _playDataManager;
    [SerializeField] private FilledBarUI _filledBarUI;
    [SerializeField] private LocomotiveCart _locomotiveCart;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _trainSpeedUI;
    [SerializeField] private LoadSceneConnector _loadSceneConnector;

    private KeyCode _pauseKey => KeyCode.Escape;

    public event Action OnGameEnd;

    private bool _isGameStart = false;
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
        
        if(!SceneManagerA.Instance._isPause && Input.GetKeyDown(_pauseKey) && _isGameStart)
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

    private void GameStart()
    {
        _isGameStart = true;
    }

    private void GameOver()
    {
        IsGameWin = false;
        GameManager.Instance.SetIsGameWin(IsGameWin);

        TrySaveData();
        _loadSceneConnector.LoadEndScene();

    }

    private void GameClear()
    {
        if (GameManager.Instance.GMode == GameMode.Tutorial)
        {
            OnGameEnd?.Invoke();
            return;
        }

        TrySaveData();

        IsGameWin = true;

        GameManager.Instance.SetIsGameWin(IsGameWin);
        _loadSceneConnector.LoadEndScene();
        
    }
    private void TrySaveData()
    {
        if (GameManager.Instance.GMode != GameMode.Tutorial)
        {
            if (SavePlayData())
            {
                GameManager.Instance.SetIsNewRecord(true);
            }
            else
            {
                GameManager.Instance.SetIsNewRecord(false);
            }
        }
    }

    // 시간 모드에서 플레이 데이터 저장
    private bool SavePlayData()
    {
        bool isNewRecord = false;

        PlayDataList playDataList = _playDataManager.LoadData();

        if (playDataList != null)
        {
            bool hasData = false;

            for (int i = 0; i < playDataList.PlayDatas.Count; i++)
            {
                PlayData playData = playDataList.PlayDatas[i];

                if (playData.GameMode == GameManager.Instance.GMode)
                {
                    if (playData.GameDifficulty == GameManager.Instance.GDifficulty)
                    {
                        hasData = true;
                        if (playData.TrainDistance < GameManager.Instance.TrainDistance)
                        {
                            isNewRecord = true;
                            playData.TrainDistance = GameManager.Instance.TrainDistance;
                            playData.PlayTime = GameManager.Instance.PlayTime;
                        }
                        else if (playData.TrainDistance == GameManager.Instance.TrainDistance
                            && playData.PlayTime > GameManager.Instance.PlayTime)
                        {
                            isNewRecord = true;
                            playData.PlayTime = GameManager.Instance.PlayTime;
                        }
                    }
                }
            }

            if (!hasData)
            {
                isNewRecord = true;
                playDataList.PlayDatas.Add(new PlayData(
                    GameManager.Instance.GMode,
                    GameManager.Instance.GDifficulty,
                    GameManager.Instance.TrainDistance,
                    GameManager.Instance.PlayTime
                ));
                isNewRecord = true;
            }
        }
        else
        {
            playDataList = new();
            playDataList.PlayDatas.Add(new PlayData(
                    GameManager.Instance.GMode,
                    GameManager.Instance.GDifficulty,
                    GameManager.Instance.TrainDistance,
                    GameManager.Instance.PlayTime
                ));
            isNewRecord = true;
        }

        if (isNewRecord) _playDataManager.SaveData(playDataList);

        return isNewRecord;
    }

    private void BindGameEvents()
    {
        _filledBarUI.OnBarLoaded += GameStart;
        _locomotiveCart.OnTrainArrived += GameOver;
        RailManager.Instance.OnRailwayConnected += GameClear;
    }

    private void UnbindGameEvents()
    {
        _filledBarUI.OnBarLoaded -= GameStart;
        _locomotiveCart.OnTrainArrived -= GameOver;
        RailManager.Instance.OnRailwayConnected -= GameClear;
    }
}
