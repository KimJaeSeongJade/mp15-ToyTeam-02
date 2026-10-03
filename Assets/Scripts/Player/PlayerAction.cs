using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 관리
/// </summary>
public class PlayerAction : MonoBehaviour
{
    private const float BASE_MOVE_SPEED = 5f;
    private const float DASH_SPEED_BONUS = 0.7f;

    private float _moveSpeed;
    private Rigidbody _playerBody;
    private PlayerHand _hand;
    private Animator _animator;
    private WaitForSeconds _waitDashDuration = new WaitForSeconds(0.3f);
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
            _animator.SetBool("IsMoving", false);
            _playerBody.velocity = Vector3.zero;
            return;
        }

        _animator.SetBool("IsMoving", true);
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
        _moveSpeed += (BASE_MOVE_SPEED * (1 + DASH_SPEED_BONUS));
        yield return _waitDashDuration;
        _moveSpeed -= (BASE_MOVE_SPEED * (1 + DASH_SPEED_BONUS));
        yield return _waitDashCooldown;
        _canDash = true;
    }

    /// <summary>
    /// 손에 든 IInteractable을 선택된 IInteractable과 바꿔 들기, 선택된 IInteractable이 없다면 내려 놓기
    /// </summary>
    /// <param name="target"> 선택된 IInteractable </param>
    public void TryButtonInteract(IInteractable target)
    {
        if (target == null)
        {
            Vector2Int playerCoord = transform.position.WorldToCoord();
            DropItem(playerCoord);
            return;
        }

        Vector2Int targetCoord = target.GameObject.transform.position.WorldToCoord();

        IInteractable newInteractable = target.ButtonInteract(_hand.Item);
        Debug.Log(newInteractable.GameObject.name);
        //bool canInteract = target.ButtonInteract(_hand.Item) != _hand.Item;

        if (newInteractable != null)
        {
            DropItem(targetCoord);
            PickUpItem(newInteractable);
        }
    }

    private void PickUpItem(IInteractable newInteractable)
    {
        _animator.SetBool("IsHolding", true);
        _hand.Item = newInteractable;
        _hand.ItemPosition = _hand.transform.position;
        _hand.ItemParent = _hand.transform;
    }

    
    private void DropItem(Vector2Int position)
    {
        _animator.SetBool("IsHolding", false);
        Map.Instance.SetHoldable(position, _hand.Item);

        if (_hand.Item != null)
        {
            _hand.ItemPosition = position.CoordToWorld();
            _hand.ItemParent = null;
            _hand.Item = null;
        }
    }

    /// <summary>
    /// 선택된 IInteractable과 자동 상호작용
    /// </summary>
    public void TryAutoInteract(IInteractable target)
    {
        if (_hand.Item == null || target == null) return;

        target.AutoInteract(_hand.Item);
    }

    private void CacheComponents()
    {
        _playerBody = GetComponent<Rigidbody>();
        _hand = GetComponentInChildren<PlayerHand>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Init()
    {
        _moveSpeed = BASE_MOVE_SPEED;
        _canDash = true;
    }
}
