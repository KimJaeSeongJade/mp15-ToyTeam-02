using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 활성화가 필요한 UI 추상 클래스
/// </summary>
public class TimerUI : UIBase
{
    // 활성화가 필요한 시간
    [SerializeField] private float _delayTime;
    // 경과 시간
    [SerializeField] private float _elapsedTime;
    // 활성화되는 게이지바
    [SerializeField] private Image _bar;
    [SerializeField] private AudioClip _audioClip1;
    [SerializeField] private AudioClip _audioClip2;
    [SerializeField] private AudioClip _audioClip3;

    private AudioPlayer UIAudioPlayer1;
    private AudioPlayer UIAudioPlayer2;
    private AudioPlayer UIAudioPlayer3;

    protected override void Awake()
    {
        base.Awake();
        _elapsedTime = 0;
        GetAudioFile();
    }
       
    private void Update()
    {
        _bar.fillAmount = _elapsedTime / _delayTime;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.layer == 6)
        {
            Debug.Log($"[사운드 시작] {gameObject.name} / {other.name} / {Time.time}");
            if (_audioClip1)
            {
                UIAudioPlayer1
                    .Init()
                    .SetClip(_audioClip1)
                    .SetLoop(false)
                    .Play();
            }
            if (_audioClip2)
            {
                UIAudioPlayer2
                    .Init()
                    .SetClip(_audioClip2)
                    .SetLoop(false)
                    .SetVolume(0.15f)
                    .Play();
            }
            if (_audioClip3)
            {
                UIAudioPlayer3
                    .Init()
                    .SetClip(_audioClip3)
                    .SetLoop(false)
                    .Play();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            
            CurrentTime();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.gameObject.layer == 6)
        {
            if (_audioClip1)
            {
                UIAudioPlayer1
                    .Stop();
            }
            if (_audioClip2)
            {
                UIAudioPlayer2
                    .Stop();
            }
            if (_audioClip3)
            {
                UIAudioPlayer3
                    .Stop();
            }
            _elapsedTime = 0;
        }
    }

    private void GetAudioFile()
    {
        if (TryGetComponent<SaveAudioFile>(out SaveAudioFile _saveAudioFile))
        {
            _audioClip1 = _saveAudioFile._audioClip[0];
            _audioClip2 = _saveAudioFile._audioClip[1];
            _audioClip3 = _saveAudioFile._audioClip[2];
        }
        UIAudioPlayer1 = AudioManager.Instance.Take();
        UIAudioPlayer2 = AudioManager.Instance.Take();
        UIAudioPlayer3 = AudioManager.Instance.Take();
    }

    protected override void PlayUI()
    {
        Debug.Log("UI활성화");
    }

    // 시간 계산
    public void CurrentTime()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _delayTime)
        {
            PlayUI();
        }
    }
}
