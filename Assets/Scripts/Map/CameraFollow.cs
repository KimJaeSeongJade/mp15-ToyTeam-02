using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;

    private void Update()
    {
        if (_targetTransform.gameObject.activeSelf == true)
        {
            transform.position = new Vector3(
            _targetTransform.position.x,
            transform.position.y,
            transform.position.z
            );
        }
    }
}
