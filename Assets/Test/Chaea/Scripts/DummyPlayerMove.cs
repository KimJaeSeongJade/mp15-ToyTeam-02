using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody _rigidbody;
    private Vector2 _direction = Vector2.zero;

    private void Awake() => CacheComponents();
    private void FixedUpdate() => Move(_direction);
    private void Update() => ReadInput();

    private void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        _direction = new Vector2(x, y);
    }

    private void Move(Vector2 input)
    {
        _rigidbody.velocity = new Vector3(
            input.x,
            0,
            input.y
        ) * _moveSpeed;
    }

    private void CacheComponents()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }
}
