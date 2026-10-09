using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBgm : MonoBehaviour
{
    [SerializeField] private AudioClip _gameBgmClip;
    [SerializeField] private FilledBarUI _barUI;

    private void Start()
    {

        if (_barUI != null)
        {
            _barUI.OnBarLoaded += playBgm;
        }

    }

    private void OnDestroy()
    {
        if (_gameBgmClip != null)
        {
            _barUI.OnBarLoaded -= playBgm;
        }
    }

    private void playBgm()
    { 
        AudioPlayer gameBgmPlayer = AudioManager.Instance.Take();
        gameBgmPlayer
            .Init()
            .SetPriority(10)
            .SetClip(_gameBgmClip)
            .Play();
    }
}
