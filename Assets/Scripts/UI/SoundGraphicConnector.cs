using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGraphicConnector : MonoBehaviour
{
    public void SetBgmVolume(float value)
    {
        SceneManagerA.Instance.SetBgmVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        SceneManagerA.Instance.SetSfxVolume(value);
    }   
}
