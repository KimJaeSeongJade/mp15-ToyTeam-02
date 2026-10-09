using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilledBarUI : MonoBehaviour
{
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private ChunkManager _chunkManager;
    [SerializeField] private WaveFunction _waveFunction;
    [SerializeField] private float _delayTime;
    [SerializeField] private Image _bar;
    [SerializeField] private float _elapsedTime;

    private bool _isInitialMapLoaded;
    private int _infiniteModeIterations = ChunkManager.CHUNK_SIZE * ChunkManager.CHUNK_SIZE;
    private int _loadedChunks;

    public event Action OnBarLoaded;

    private void Awake()
    {
        _elapsedTime = 0;
    }

    private void OnEnable() => BindMapLoadedEvents();

    private void Update()
    {
        CurrentTime();
        UpdateBar();
        CheckMapLoad();
    }

    private void OnDisable() => UnbindMapLoadedEvents();

    public void CurrentTime()
    {
        _elapsedTime += Time.deltaTime;
    }

    private void UpdateBar()
    {
        if (_mapLoader.GameMode == GameMode.Infinite)
        {
            _bar.fillAmount = (float)(_waveFunction.Iterations + _waveFunction.ChunkIndex * _infiniteModeIterations) /
                (_infiniteModeIterations * 3);
        }
        else if (_mapLoader.GameMode == GameMode.Quick)
        {
            _bar.fillAmount = (float)(_waveFunction.Iterations + _waveFunction.ChunkIndex * _infiniteModeIterations) /
                (_infiniteModeIterations * 2);
        }
        else
        {
            _bar.fillAmount = _elapsedTime / _delayTime;
        }
    }

    private void CheckMapLoad()
    {
        if (!_isInitialMapLoaded || _bar.fillAmount < 1) return;

        OnBarLoaded?.Invoke();
        GameManager.Instance.PlayStart();
        gameObject.SetActive(false);
    }

    private void OnInitialMapLoaded()
    {
        _isInitialMapLoaded = true;
    }

    private void BindMapLoadedEvents()
    {
        _chunkManager.OnInitialMapLoaded += OnInitialMapLoaded;
    }

    private void UnbindMapLoadedEvents()
    {
        _chunkManager.OnInitialMapLoaded -= OnInitialMapLoaded;
    }
}