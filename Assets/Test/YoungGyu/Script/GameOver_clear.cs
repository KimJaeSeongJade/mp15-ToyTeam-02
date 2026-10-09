using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameOver_clear : MonoBehaviour
{ 
    [SerializeField] private AudioClip _gameOverClip;
    [SerializeField] private AudioClip _gameClearClip;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += PlayBgm;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= PlayBgm;
        }
    }

    private void PlayBgm(bool isWIn)
    {

        AudioClip playClip = isWIn ? _gameClearClip : _gameOverClip;

        AudioPlayer endPlay = AudioManager.Instance.Take();
        endPlay
            .Init()
            .SetClip(playClip)
            .SetLoop(false)
            .Play();
    }
    
    

}
