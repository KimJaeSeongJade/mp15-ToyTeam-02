using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    // 플레이 시간
    private float _playTime;
    public float PlayTime
    {
        get { return _playTime; }
        set { _playTime = value; }
    }
    // 열차의 속도
    private float _trainSpeed;
    //게임이 Pasue상태인지
    private bool _isPause;
    public bool IsPause
    {
        get { return _isPause; }
        set { _isPause = value; }
    }

    //--------------------
    private void Awake()
    {
        Init();
        SetSingleton();
    }

    private void Update()
    {
        //게임씬이고 게임 진행중일 때
        if (SceneManager.GetActiveScene().buildIndex == 2 && !_isPause)
        {
            PlayStart();
        }
    } 

    // 플레이 시간 계산
    private void PlayStart()
    {
        _playTime += Time.deltaTime;
        Debug.Log(_playTime);
    }

    private void Init()
    {
        _isPause = false;
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
