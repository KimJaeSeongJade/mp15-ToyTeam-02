using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class LocomotiveCart : Train
{
    public event Action OnTrainArrived;
    private const int TRAIN_SPEED_SCALE = 1000;
    
    

    protected override void Init()
    {
        /*
        AudioPlayer Locomotive = AudioManager.Instance.Take();

        Locomotive
            .Init()
            .SetClip(_locomotiveClip)
            .SetPriority(50)
            .Play();
        AudioPlayer LocomotiveHont = AudioManager.Instance.Take();
        LocomotiveHont
            .Init()
            .SetClip(_locomotiveHontClip)
            .SetPriority(50)
            .SetPlayOnAwake(true);
        */
        base.Init();
    }

    public override void TrainDepart()
    {
        base.TrainDepart();
        GameManager.Instance.SetTrainSpeed(MaxTrainSpeed);
    }

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
