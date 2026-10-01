using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 관리
/// </summary>
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

    /// <summary>
    /// 방향을 전달 받아 플레이어를 해당 방향으로 전진
    /// </summary>
    /// <param name="direction"> 플레이어가 이동할 방향 </param>
    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            _playerBody.velocity = Vector3.zero;
            return;
        }

        _playerBody.rotation = Quaternion.LookRotation(direction);
        _playerBody.velocity = _playerBody.transform.forward * _moveSpeed;
    }

    /// <summary>
    /// 플레이어가 이동 방향으로 대쉬
    /// </summary>
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

    /// <summary>
    /// 선택된 IInteractable와 손에 들고 있는 IInteractable를 전달 받아 선택된 IInteractable 상호작용 메서드를 호출
    /// </summary>
    /// <param name="target"> 선택된 IInteractable </param>
    /// <param name="handItem"> 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable TryInteract(IInteractable target, IInteractable handItem)
    {
        return target.ButtonInteract(handItem);
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
