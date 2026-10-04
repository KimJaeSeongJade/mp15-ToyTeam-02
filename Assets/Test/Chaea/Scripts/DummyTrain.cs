using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DummyTrain : MonoBehaviour
{
    [SerializeField] private SplineAnimate _splineAnimate;

    public GameObject GameObject { get; }

    public BlockType BlockType { get; }

    private void Awake()
    {
        _splineAnimate.enabled = false;
    }

    private void OnEnable() => Init();
    private void OnDisable() => UnbindSplineAnimateEvents();

    private void OnSplineUpdate(Vector3 vector, Quaternion quaternion)
    {
        if (_splineAnimate.NormalizedTime >= 1.0f)
        {
            Debug.Log("열차 끝에 도착");
        }
    }

    private void Init()
    {
        BindSplineAnimateEvents();
        _splineAnimate.enabled = true;
        _splineAnimate.Restart(true);
    }

    private void BindSplineAnimateEvents()
    {
        _splineAnimate.Updated += OnSplineUpdate;
    }

    private void UnbindSplineAnimateEvents()
    {
        _splineAnimate.Updated -= OnSplineUpdate;
    }
}
