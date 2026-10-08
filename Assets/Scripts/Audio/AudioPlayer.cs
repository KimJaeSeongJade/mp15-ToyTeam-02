using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnEnable() => Init();
    private void Update() => WaitForEnd();

    private void Init()
    {
        _audioSource.volume = 1f;
        _audioSource.playOnAwake = true;
        _audioSource.loop = false;
        _audioSource.clip = null;
    }

    private void WaitForEnd()
    {
        if (_audioSource.isPlaying || _audioSource.loop) return;
        Stop();
    }

    public AudioPlayer SetVolume(float volume)
    {
        _audioSource.volume = volume;
        return this;
    }

    public AudioPlayer SetPlayOnAwake(bool playOnAwake)
    {
        _audioSource.playOnAwake = playOnAwake;
        return this;
    }

    public AudioPlayer SetLoop(bool loop)
    {
        _audioSource.loop = loop;
        return this;
    }

    public AudioPlayer SetClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        return this;
    }

    public void Play()
    {
        _audioSource.Play();
    }

    public void Pause()
    {
        _audioSource.Pause();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void ReturnToPool()
    {
        AudioManager.Instance.Return(this);
    }
}
