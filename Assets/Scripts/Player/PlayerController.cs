using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 입력과 주변 오브젝트 감지 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    private const float THRESHOLD = 0.7f;

    [SerializeField] private PlayerHand _playerHand;
    [SerializeField] private PlayerAction _player;
    [SerializeField] private DetectRange _detectRange;
    private Vector3 _direction;
    private List<IInteractable> _detecteds => _detectRange.Detecteds;
    private IInteractable _target;
    private KeyCode _moveUp = KeyCode.W;
    private KeyCode _moveDown = KeyCode.S;
    private KeyCode _moveLeft = KeyCode.A;
    private KeyCode _moveRight = KeyCode.D;
    private KeyCode _dashKey = KeyCode.LeftShift;
    private KeyCode _interactKey = KeyCode.Space;
    private bool _isPressedMoveKey => Input.GetKey(_moveUp) || Input.GetKey(_moveDown) || Input.GetKey(_moveLeft) || Input.GetKey(_moveRight);
    private bool _isPressedDashKey => Input.GetKeyDown(_dashKey);
    private bool _isPressedInteractKey => Input.GetKeyDown(_interactKey);

    // ------------------------------
    private void FixedUpdate()
    {
        _player.Move(_direction);
    }
    private void Update()
    {
        ReadMove();
        ReadDash();
        TryDetectInteractable();
        ReadInteract();
        AutoInteract();
    }
    private void OnDrawGizmos()
    {
        if (_detectRange == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _detectRange.Range);
        float angle = Mathf.Acos(THRESHOLD) * Mathf.Rad2Deg;

        Vector3 leftDir = Quaternion.Euler(0f, -angle, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, angle, 0f) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftDir * _detectRange.Range);
        Gizmos.DrawRay(transform.position, rightDir * _detectRange.Range);
    }
    // ------------------------------

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

    private void ReadDash()
    {
        if (!_isPressedDashKey) return;

        _player.Dash();
    }
    
    private void TryDetectInteractable()
    {
        List<IInteractable> canTargetList = new();
        Dictionary<IInteractable, float> canTargetDict = new();

        foreach (IInteractable detected in _detecteds)
        {
            Debug.Log(detected.GameObject.name);
            Vector3 playerDirection = transform.forward;
            Vector3 toDetectedDirection = (detected.GameObject.transform.position - transform.position).normalized;
            float lookPercentage = Vector3.Dot(toDetectedDirection, playerDirection);
            Debug.Log(lookPercentage);

            Ray ray = new Ray(transform.position, toDetectedDirection);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, _detectRange.Range)) continue;
            
            if (lookPercentage >= THRESHOLD && hit.transform.GetComponent<IInteractable>() == detected && detected != _playerHand.Item)
            {
                if (!canTargetList.Contains(detected)) canTargetList.Add(detected);
                canTargetDict.TryAdd(detected, lookPercentage);
            }
            else
            {
                if (canTargetList.Contains(detected)) canTargetList.Remove(detected);
                if (canTargetDict.ContainsKey(detected)) canTargetDict.Remove(detected);
            }
        }

        IInteractable target = null;
        float targetLookPercentage = -1;
        foreach(IInteractable canTarget in canTargetList)
        {
            float canTargetLookPercentage = canTargetDict[canTarget];
            if(canTargetLookPercentage > targetLookPercentage)
            {
                target = canTarget;
                targetLookPercentage = canTargetLookPercentage;
            }
        }


        // 비활성화된 오브젝트를 타겟에서 제외
        if (target == _playerHand.Item || target != null && target.GameObject.activeSelf == false)
        {
            target = null;
        }

        if (target == null)
        {
            Vector2Int playerCoord = transform.position.WorldToCoord();
            target = Map.Instance.GetHoldable(playerCoord);
        }

        if (_target != target)
        {
            _target?.Untargeted();
            _target = target;
            _target?.Targeted();
        }
    }

    private void ReadInteract()
    {
        if (!_isPressedInteractKey) return;

        _player.TryButtonInteract(_target);
    }

    private void AutoInteract()
    {
        _player.TryAutoInteract(_target);
    }
}
