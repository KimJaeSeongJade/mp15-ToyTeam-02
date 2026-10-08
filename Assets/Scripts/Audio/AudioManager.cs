using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Stack<AudioPlayer> _audioPlayerStack;

    private void Awake() => SetSingleton();

    public AudioPlayer Take()
    {
        return _audioPlayerStack.Pop();
    }

    public void Return(AudioPlayer audioPlayer)
    {
        _audioPlayerStack.Push(audioPlayer);
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
