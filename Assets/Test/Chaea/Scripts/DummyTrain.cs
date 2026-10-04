using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DummyTrain : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] protected SplineAnimate _splineAnimate;
    [SerializeField] private float _offsetRate = 6f;

    public static float MaxTrainSpeed = .5f;

    private WaitForSeconds _wait = new WaitForSeconds(2f);

    // -----------------------------
    private void Awake() => Init();
    private void OnEnable() => StartCoroutine(TrainDepartRoutine());
    private void OnDestroy() => UnbindRailEvents();
    // -----------------------------

    protected virtual void OnSplineUpdate(Vector3 vector, Quaternion quaternion)
    {
        if (_splineAnimate.NormalizedTime >= 1.0f - _splineAnimate.StartOffset)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        _splineAnimate.enabled = false;
        BindRailEvents();
    }

    private IEnumerator TrainDepartRoutine()
    {
        Debug.Log(_splineManager.SplineCount);
        _splineAnimate.StartOffset = _offsetRate / _splineManager.SplineCount;
        _splineAnimate.enabled = true;
        _splineAnimate.Restart(true);
        _splineAnimate.MaxSpeed = 0f;

        yield return _wait;
        TrainDepart();
    }

    private void TrainDepart()
    {
        _splineAnimate.MaxSpeed = MaxTrainSpeed;
        _splineAnimate.Restart(true);
    }

    private void UpdateOffset(int splineCount)
    {
        _splineAnimate.StartOffset = _offsetRate / splineCount;
    }

    private void OnRailwayConnected()
    {
        Debug.Log("railwayConnected");
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
