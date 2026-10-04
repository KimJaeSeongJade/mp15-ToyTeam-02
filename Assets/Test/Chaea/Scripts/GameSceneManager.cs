using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [SerializeField] private LocomotiveCart _locomotiveCart;

    public event Action OnGameEnd;

    public bool IsGameEnd { get; private set; }
    public bool IsGameWin { get; private set; }


    private void Awake() => SetSingleton();
    private void Start() => BindTrainEvents();
    private void OnDestroy() => UnbindTrainEvents();

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

    private void GameOver()
    {
        Debug.Log("Game over");
        // OnGameEnd?.Invoke();
        IsGameEnd = true;
        IsGameWin = false;
    }

    private void GameWin()
    {
        // OnGameEnd?.Invoke();
        IsGameEnd = true;
        IsGameWin = true;
    }

    private void BindTrainEvents()
    {
        _locomotiveCart.OnTrainArrived += GameOver;
    }

    private void UnbindTrainEvents()
    {
        _locomotiveCart.OnTrainArrived -= GameOver;
    }
}
