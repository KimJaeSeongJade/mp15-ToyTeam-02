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

    protected override void Awake()
    {
        base.Awake();
        _elapsedTime = 0;
    }
    private void Update()
    {
        _bar.fillAmount = _elapsedTime / _delayTime;
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
            _elapsedTime = 0;
        }
    }

    public override void PlayUI()
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
