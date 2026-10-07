using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game관리 Singleton Manager
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance 
    { 
        get 
        {
            if(_instance == null )
            {
                _instance = _instance = FindObjectOfType <GameManager> ();
                DontDestroyOnLoad( _instance.gameObject );
            }
            return _instance;
        } 
    }

    // 게임 모드
    public GameMode GMode { get; private set; }

    // 플레이 시간
    public float PlayTime { get; private set; }
    
    // 열차의 속도
    public float TrainSpeed { get; private set; }

    //[SerializeField] private float TrainSpeed;

    // 게임이 Pasue상태인지 
    public bool IsPaused { get; private set; }

    // 게임을 클리어 했는지
    public bool IsGameWin; //{ get; private set; }

    //--------------------
    private void Awake()
    {        
        SetSingleton();
        Init();
    }

    //private void Start()
    //{
    //    SetTrainSpeed(TrainSpeed);
    //}


    private void Update()
    {
        //게임씬이고 게임 진행중일 때
        //if (SceneManager.GetActiveScene().buildIndex == 2 && !IsPaused)
        //{
        //    PlayStart();
        //}
    } 
    
    public void SetGameMode(GameMode gMode)
    {
        GMode = gMode;
    }

    public void IsPause(bool value)
    {
        IsPaused = value;
    }

    public void SetIsGameWin(bool value)
    {
        IsGameWin = value;
    }

    // 플레이 시간 리셋
    public void ResetPlayTime()
    {
        PlayTime = 0f;
    }

    // 플레이 시간 계산
    private void PlayStart()
    {
        PlayTime += Time.deltaTime;
        //Debug.Log(PlayTime);
    }

    // 열차 속도
    public void SetTrainSpeed(float value)
    {
        TrainSpeed = value;
    }

    private void Init()
    {
        IsPause(false);
    }

    private void SetSingleton()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<GameManager>();
        DontDestroyOnLoad(gameObject);
    }
}
