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
            Debug.Log("이벤트 구독 완료! 아직 노래 안 틈 대기 중...");
        }
        else
        {
            Debug.LogError("FilledBarUI를 찾을 수 없습니다!");
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
        Debug.Log("로딩 종료 신호 받음! BGM 재생 시작!"); // 콘솔창에서 언제 뜨는지 확인해 보세요!
        AudioPlayer gameBgmPlayer = AudioManager.Instance.Take();
        gameBgmPlayer
            .Init()
            .SetPriority(10)
            .SetClip(_gameBgmClip)
            .Play();
    }
}
