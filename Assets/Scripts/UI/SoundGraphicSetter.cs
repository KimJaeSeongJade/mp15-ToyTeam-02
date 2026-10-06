using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundGraphicSetter : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void OnEnable()
    {
        _bgmSlider.value = SceneManagerA.Instance.BgmVolume;
        _sfxSlider.value = SceneManagerA.Instance.SfxVolume;
    }

    public void ChangeBgmVolume(float value)
    {
        SceneManagerA.Instance.SetBgmVolume(value);
    }

    public void ChangeSfxVolume(float value)
    {
        SceneManagerA.Instance.SetSfxVolume(value);
    }
}
