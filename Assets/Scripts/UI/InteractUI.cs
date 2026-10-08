using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용키를 누르면 작동하는 UI
/// </summary>
public class InteractUI : UIBase
{
    [SerializeField] private AudioClip _audioClip;
    private KeyCode _interactKey => KeyCode.Space;

    // Collider에 올라왔는지 확인하는 bool
    private bool _isOnTrigger;

    private void Awake()
    {
        GetAudioFile();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!_audioClip)
        {
            AudioPlayer UIAudioPlayer = AudioManager.Instance.Take();

            UIAudioPlayer
                .Init()
                .SetClip(_audioClip)
                .SetLoop(false)
                .Play();
        }


        base.OnTriggerEnter(other);
        if (other.gameObject.layer == 6)
        {
            _isOnTrigger = true;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.gameObject.layer == 6)
        {
            _isOnTrigger = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _isOnTrigger = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_interactKey) && _isOnTrigger)
        {
            PlayUI();
        }
    }

    private void GetAudioFile()
    {
        if (TryGetComponent<SaveAudioFile>(out SaveAudioFile _saveAudioFile))
        {
            _audioClip = _saveAudioFile._audioClip;
        }
    }

    protected override void PlayUI()
    {
        Debug.Log("UI활성화");
    }
}
