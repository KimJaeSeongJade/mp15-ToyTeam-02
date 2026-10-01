using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerAction _player;
    private Vector3 _direction;
    private KeyCode _dashKey = KeyCode.LeftShift;
    private bool _isPressedDashKey => Input.GetKeyDown(_dashKey);

    // ------------------------------
    private void Awake() => CacheComponents();
    private void Update()
    {
        ReadMove();
        ReadDash();
    }
    private void FixedUpdate()
    {
        _player.Move(_direction);
    }
    // ------------------------------

    private void ReadMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _direction = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void ReadDash()
    {
        if (!_isPressedDashKey) return;

        _player.Dash();
    }

    private void CacheComponents()
    {
        _player = GetComponent<PlayerAction>();
    }
}
