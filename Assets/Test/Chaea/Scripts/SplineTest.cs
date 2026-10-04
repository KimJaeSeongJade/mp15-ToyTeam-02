using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private DummyTrain _dummyTrain;

    private Camera _cam;

    private void Awake() => CacheComponents();

    private void Update()
    {
        ReadMouseInput();
    }

    private void ReadMouseInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (_dummyTrain.gameObject.activeSelf == false)
            {
                _dummyTrain.gameObject.SetActive(true);
            }
            else
            {
                _dummyTrain.gameObject.SetActive(false);
            }
        }
    }

    private void CacheComponents()
    {
        _cam = Camera.main;
    }
}
