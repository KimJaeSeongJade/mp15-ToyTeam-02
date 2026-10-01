using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private const float BASE_MOVE_SPEED = 5f;
    private const float DASH_SPEED_BONUS = 4f;

    private Rigidbody _playerBody;
    private float _moveSpeed;
    private WaitForSeconds _waitDashDuration = new WaitForSeconds(0.2f);
    private WaitForSeconds _waitDashCooldown = new WaitForSeconds(1f);
    private bool _canDash;

    // ------------------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    // ------------------------------

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _playerBody.velocity = Vector3.zero;
            return;
        }

        _playerBody.MoveRotation(Quaternion.LookRotation(direction));
        _playerBody.velocity = _playerBody.transform.forward * _moveSpeed;
    }

    public void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        if (!_canDash) yield break;

        _canDash = false;
        _moveSpeed += (BASE_MOVE_SPEED * DASH_SPEED_BONUS);
        yield return _waitDashDuration;
        _moveSpeed -= (BASE_MOVE_SPEED * DASH_SPEED_BONUS);
        yield return _waitDashCooldown;
        _canDash = true;
    }

    private void CacheComponents()
    {
        _playerBody = GetComponent<Rigidbody>();
    }

    private void Init()
    {
        _moveSpeed = BASE_MOVE_SPEED;
        _canDash = true;
    }
}
