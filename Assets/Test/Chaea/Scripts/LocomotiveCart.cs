using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LocomotiveCart : Train
{
    public event Action OnTrainArrived;

    protected override void OnSplineUpdate(Vector3 vector, Quaternion quaternion)
    {
        base.OnSplineUpdate(vector, quaternion);
        if (_splineAnimate.NormalizedTime >= 1.0f - _splineAnimate.StartOffset)
        {
            OnTrainArrived?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
