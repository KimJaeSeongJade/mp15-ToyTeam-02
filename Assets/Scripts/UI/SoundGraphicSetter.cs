using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundGraphicSetter : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Start()
    {
        _bgmSlider.minValue = 0f;
        _bgmSlider.maxValue = 100f;

        _sfxSlider.minValue = 0f;
        _sfxSlider.maxValue = 100f;

        _bgmSlider.value = SceneManagerA.Instance.BgmVolume;
        _sfxSlider.value = SceneManagerA.Instance.SfxVolume;
    }

    //private void OnEnable()
    //{
    //    float bgmVolume = SceneManagerA.Instance.BgmVolume;
    //    float sfxVolume = SceneManagerA.Instance.SfxVolume;

    //    //_bgmSlider.value = SceneManagerA.Instance.BgmVolume;
    //    //_sfxSlider.value = SceneManagerA.Instance.SfxVolume;

    //    Debug.Log($"BGM: {bgmVolume}, SFX: {sfxVolume}");

    //    _bgmSlider.minValue = 0f;
    //    _bgmSlider.maxValue = 100f;

    //    _sfxSlider.minValue = 0f;
    //    _sfxSlider.maxValue = 100f;

    //    _bgmSlider.SetValueWithoutNotify(bgmVolume);
    //    _sfxSlider.SetValueWithoutNotify(sfxVolume);
    //}

    public void ChangeBgmVolume(float value)
    {
        SceneManagerA.Instance.SetBgmVolume(value);
    }

    public void ChangeSfxVolume(float value)
    {
        SceneManagerA.Instance.SetSfxVolume(value);
    }
}
