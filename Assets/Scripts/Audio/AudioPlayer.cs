using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void Update() => WaitForEnd();

    /// <summary>
    /// AudioPlayer 초기화
    /// </summary>
    /// <returns></returns>
    public AudioPlayer Init()
    {
        _audioSource.volume = .5f;
        _audioSource.priority = 128;
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        _audioSource.clip = null;
        return this;
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

    public AudioPlayer SetPriority(int priority)
    {
        _audioSource.priority = priority;
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
