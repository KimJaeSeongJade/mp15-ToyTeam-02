using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 입력과 주변 오브젝트 감지 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    private const float THRESHOLD = 0.7f;

    private PlayerAction _player;
    private Vector3 _direction;
    private DetectRange _detectRange;
    private List<IInteractable> _detecteds => _detectRange.Detecteds;
    private List<IInteractable> _canTargetList;
    private Dictionary<IInteractable, float> _canTargetDict;
    private IInteractable _target;
    private KeyCode _dashKey = KeyCode.LeftShift;
    private bool _isPressedDashKey => Input.GetKeyDown(_dashKey);

    // ------------------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    private void Update()
    {
        ReadMove();
        ReadDash();
        DetectInteractable();
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

    private void DetectInteractable()
    {
        foreach (IInteractable detected in _detecteds)
        {
            Vector3 playerDirection = transform.forward;
            Vector3 toDetectedDirection = (detected.GameObject.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position, toDetectedDirection);
            RaycastHit hit;

            float lookPercentage = Vector3.Dot(toDetectedDirection, playerDirection);
            if (!Physics.Raycast(ray, out hit, _detectRange.Range)) continue;
            if (lookPercentage >= THRESHOLD && hit.transform.GetComponent<IInteractable>() == detected)
            {
                if (!_canTargetList.Contains(detected)) _canTargetList.Add(detected);
                _canTargetDict.TryAdd(detected, lookPercentage);
            }
            else
            {
                if (_canTargetList.Contains(detected)) _canTargetList.Remove(detected);
                if (_canTargetDict.ContainsKey(detected)) _canTargetDict.Remove(detected);
            }
        }

        IInteractable target = null;
        float targetLookPercentage = -1;
        for (int i = 0; i < _canTargetList.Count; i++)
        {
            IInteractable canTarget = _canTargetList[i];
            float canTargetLookPercentage = _canTargetDict[_canTargetList[i]];
            if (canTargetLookPercentage > targetLookPercentage)
            {
                target = canTarget;
                targetLookPercentage = canTargetLookPercentage;
            }
        }

        if (_target != target)
        {
            _target?.Untargeted();
            _target = target;
            _target?.Targeted();
        }
    }

    private void CacheComponents()
    {
        _player = GetComponent<PlayerAction>();
        _detectRange = GetComponentInChildren<DetectRange>();
    }

    private void Init()
    {
        _canTargetList = new();
        _canTargetDict = new();
    }
}
