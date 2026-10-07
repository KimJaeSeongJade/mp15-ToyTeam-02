using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarftCartUIFollow : MonoBehaviour
{
    [SerializeField] private Transform _carftTransform;
    private Transform _cameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if(_carftTransform == null ||  _cameraTransform == null) return;
        
        _carftTransform.rotation = _cameraTransform.rotation;
    }
}
