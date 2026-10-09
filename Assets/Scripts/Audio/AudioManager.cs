using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject _audioPlayerPrefab;
    private Stack<AudioPlayer> _audioPlayerStack  = new Stack<AudioPlayer>();

    private void Awake() => SetInstance();

    public AudioPlayer Take()
    {
        if (_audioPlayerStack.Count > 0)
        {
            return _audioPlayerStack.Pop();
        }
        else
        {
            GameObject newAudioManager = Instantiate(_audioPlayerPrefab);
            return newAudioManager.GetComponent<AudioPlayer>();
        }
    }

    public void Return(AudioPlayer audioPlayer)
    {
        _audioPlayerStack.Push(audioPlayer);
    }

    private void SetInstance()
    {
        Instance = this;
    }
}