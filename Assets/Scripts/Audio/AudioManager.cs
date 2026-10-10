using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject _audioPlayerPrefab;
    [SerializeField] private AudioMixerGroup _bgmG;
    public AudioMixerGroup BgmG=> _bgmG;
    [SerializeField] private AudioMixerGroup _sfxG;
    public AudioMixerGroup SfxG => _sfxG;
    private Stack<AudioPlayer> _audioPlayerStack  = new Stack<AudioPlayer>();

    

    private void Awake() => SetInstance();

    public AudioPlayer Take(bool isBgm = false)
    {
        AudioPlayer audioPlayer;

        if (_audioPlayerStack.Count > 0)
        {
            return _audioPlayerStack.Pop();
        }
        else
        {
            GameObject newAudioManager = Instantiate(_audioPlayerPrefab);
            audioPlayer = newAudioManager.GetComponent<AudioPlayer>();
            //return newAudioManager.GetComponent<AudioPlayer>();
        }

        audioPlayer.SetMixerGroup(isBgm ? _bgmG : _sfxG);

        return audioPlayer;
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