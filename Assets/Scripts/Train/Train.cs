using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Train : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] protected SplineAnimate _splineAnimate;
    [SerializeField] private float _offsetRate = 6f;
    [SerializeField] private float _locomotiveCartOffsetRate = 6f;

    /// <summary>
    /// 열차 속도
    /// </summary>
    public static float MaxTrainSpeed = 0.1f;

    private WaitForSeconds _wait = new WaitForSeconds(5f);
    private bool _isRailwayConnected;

    // -----------------------------
    private void Awake() => Init();

    protected virtual void OnEnable()
    {
        _splineAnimate.StartOffset = _offsetRate / _splineManager.SplineCount;
        _splineAnimate.enabled = true;
        _splineAnimate.Restart(true);
        _splineAnimate.MaxSpeed = 0f;
    }

    private void OnDestroy() => UnbindRailEvents();
    // -----------------------------

    // Railway가 연결되면 멈추고, 연결되지 않으면 파괴
    protected virtual void OnSplineUpdate(Vector3 vector, Quaternion quaternion)
    {
        if (_isRailwayConnected)
        {
            if (_splineAnimate.NormalizedTime >= 1.0f -
                _locomotiveCartOffsetRate / _splineManager.SplineCount - 0.01f)
            {
                _splineAnimate.Pause();
            }
        }
        else
        {
            if (_splineAnimate.NormalizedTime >= 1.0f - _splineAnimate.StartOffset)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual void Init()
    {
        if (GameManager.Instance.TrainSpeed != 0) MaxTrainSpeed = GameManager.Instance.TrainSpeed;
        _splineAnimate.enabled = false;
        BindRailEvents();
    }

    /// <summary>
    /// 열차가 5초 기다렸다가 출발
    /// </summary>
    public void StartTrainDepartRoutine()
    {
        StartCoroutine(TrainDepartRoutine());
    }

    private IEnumerator TrainDepartRoutine()
    {
        yield return _wait;
        TrainDepart();
    }

    /// <summary>
    /// 열차가 최고 속도로 출발
    /// </summary>
    public virtual void TrainDepart()
    {
        _splineAnimate.MaxSpeed = MaxTrainSpeed;
        Debug.Log($"Train : TrainDepart train speed {_splineAnimate.MaxSpeed}");
        _splineAnimate.Restart(true);
    }

    private void UpdateOffset(int splineCount)
    {
        _splineAnimate.StartOffset = _offsetRate / splineCount;
    }

    private void OnRailwayConnected()
    {
        _isRailwayConnected = true;
    }

    private void BindRailEvents()
    {
        _splineAnimate.Updated += OnSplineUpdate;
        _splineManager.OnRailwayChanged += UpdateOffset;
        RailManager.Instance.OnRailwayConnected += OnRailwayConnected;
    }

    private void UnbindRailEvents()
    {
        _splineAnimate.Updated -= OnSplineUpdate;
        _splineManager.OnRailwayChanged -= UpdateOffset;
        RailManager.Instance.OnRailwayConnected -= OnRailwayConnected;
    }
}