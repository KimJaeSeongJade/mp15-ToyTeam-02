using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    
    [SerializeField] private Rigidbody _playerBody;
    [SerializeField] private Animator _animator;

    private const float BASE_MOVE_SPEED = 5f;
    private float _moveSpeed;
    private Vector3 _direction;

    private KeyCode _moveUp = KeyCode.W;
    private KeyCode _moveDown = KeyCode.S;
    private KeyCode _moveLeft = KeyCode.A;
    private KeyCode _moveRight = KeyCode.D;

    private bool _isPressedMoveKey => Input.GetKey(_moveUp) || Input.GetKey(_moveDown) || Input.GetKey(_moveLeft) || Input.GetKey(_moveRight);

    //---------------------
    private void Awake() => Init();

    private void FixedUpdate()
    {
        Move(_direction);
    }

    private void Update()
    {
        ReadMove();
    }

    private void Init()
    {
        _moveSpeed = BASE_MOVE_SPEED;
        _playerBody = GetComponent<Rigidbody>();
    }

    private void ReadMove()
    {
        if (!_isPressedMoveKey)
        {
            _direction = Vector3.zero;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _direction = new Vector3(horizontal, 0f, vertical).normalized;
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _animator.SetBool("IsMoving", false);
            _playerBody.velocity = Vector3.zero;
            return;
        }

        _animator.SetBool("IsMoving", true);
        _playerBody.rotation = Quaternion.LookRotation(direction);
        _playerBody.velocity = _playerBody.transform.forward * _moveSpeed;
    }
}
