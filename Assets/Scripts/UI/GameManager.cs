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

    // 열차의 이동 거리
    public int TrainDistance { get; private set; }

    //[SerializeField] private float TrainSpeed;

    // 게임이 Pause상태인지 
    public bool IsPaused { get; private set; }

    // 게임을 클리어 했는지
    public bool IsGameWin; //{ get; private set; }
    public bool IsNewRecord { get; private set; }
    private bool _isPlaying;
    //--------------------
    private void Awake()
    {        
        SetSingleton();
        Init();
    }

//    private void Start()
//    {
//#if !UNITY_EDITOR && UNITY_WEBGL
//        WebGLInput.stickyCursorLock = false;
//#endif
//    }


    private void Update()
    {         
        // 게임씬이고 게임 진행중일 때
        if (SceneManager.GetActiveScene().buildIndex == 4 && !IsPaused && _isPlaying)
        {
            PlayTime += Time.deltaTime;
        }
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
        _isPlaying = false;
    }

    // 플레이 시간 리셋
    public void ResetPlayTime()
    {
        PlayTime = 0f;
    }

    // 플레이 시간 계산
    public void PlayStart()
    {
        ResetPlayTime();
        _isPlaying = true;
    }

    // 열차 속도
    public void SetTrainSpeed(float value)
    {
        TrainSpeed = value;
    }

    // 열차 이동 거리
    public void SetTrainDistance(int value)
    {
        TrainDistance = value;
    }

    // 신기록
    public void SetIsNewRecord(bool value)
    {
        IsNewRecord = value;
    }

    private void Init()
    {
        IsPause(false);
        TrainSpeed = 0.1f;
        //CursorLock();
    }

    //private void CursorLock()
    //{
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //}

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
