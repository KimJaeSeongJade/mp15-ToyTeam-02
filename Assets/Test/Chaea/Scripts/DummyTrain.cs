using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DummyTrain : MonoBehaviour
{
    [SerializeField] protected SplineAnimate _splineAnimate;
    private WaitForSeconds _wait = new WaitForSeconds(2f);

    // -----------------------------
    private void Awake()
    {
        _splineAnimate.enabled = false;
    }

    private void OnEnable() => Init();
    private void OnDisable() => UnbindSplineAnimateEvents();
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
        BindSplineAnimateEvents();

        StartCoroutine(TrainDepartRoutine());
    }

    private IEnumerator TrainDepartRoutine()
    {
        _splineAnimate.enabled = true;
        _splineAnimate.Restart(true);
        _splineAnimate.MaxSpeed = 0f;

        yield return _wait;
        TrainDepart();
    }

    private void TrainDepart()
    {
        _splineAnimate.MaxSpeed = .2f;
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
