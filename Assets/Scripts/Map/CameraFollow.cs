using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _trainTransform;
    [SerializeField] private Transform _playerTransform;

    private Transform _targetTransform;

    private void Start() => Init();
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

    public void ChangeTarget()
    {
        if (_targetTransform == _trainTransform) _targetTransform = _playerTransform;
        else _targetTransform = _trainTransform;
    }

    private void Init()
    {
        _targetTransform = _trainTransform;
    }
}
