using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 관리
/// </summary>
public class PlayerAction : MonoBehaviour
{
    private const float BASE_MOVE_SPEED = 5f;
    private const float DASH_SPEED_BONUS = 0.5f;
    private const string PARAM_IS_MOVING = "IsMoving";
    private const string PARAM_IS_HOLDING = "IsHolding";
    private const string PARAM_IS_HOLDING_TOOL = "IsHoldingTool";
    private const string PARAM_IS_USING = "IsUsing";

    [SerializeField] private Rigidbody _playerBody;
    [SerializeField] private PlayerHand _hand;
    [SerializeField] private Animator _animator;
    [SerializeField] private DetectRange _detectRange;

    private float _moveSpeed;
    private WaitForSeconds _waitDashDuration = new WaitForSeconds(0.3f);
    private WaitForSeconds _waitDashCooldown = new WaitForSeconds(1f);
    private bool _canDash;

    // ------------------------------
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
            _animator.SetBool(PARAM_IS_MOVING, false);
            _playerBody.velocity = Vector3.zero;
            return;
        }

        _animator.SetBool(PARAM_IS_MOVING, true);
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


        IInteractable newTarget = target.ButtonInteract(_hand.Item);
        bool canInteract = newTarget != _hand.Item;

        if (canInteract)
        {
            if (newTarget != target)
            {
                PickUpItem((newTarget));
                _animator.SetLayerWeight(1, 0f);
                _animator.SetBool(PARAM_IS_HOLDING_TOOL, false);
                _animator.SetBool(PARAM_IS_HOLDING, false);
            }
            else
            {
                DropItem(targetCoord);
                PickUpItem(target);
            }
        }
    }

    private void PickUpItem(IInteractable newInteractable)
    {
        // 제거했으니 로드된 청크에서 제거
        Map.Instance.RemoveLoadedPoolable(newInteractable as IPoolable);

        _hand.Item = newInteractable;
        if (newInteractable == null) return;

        _animator.SetLayerWeight(1, 1f);
       
        _hand.Item.GameObject.transform.rotation = transform.rotation;

        // 도구면 회전
        ToolBase tool = newInteractable as ToolBase;
        if (tool != null)
        {
            _animator.SetBool(PARAM_IS_HOLDING_TOOL, true);
            _hand.Item.GameObject.transform.Rotate(-45f, -90f, 0);
            tool.ToolUI(false);
        }
        else
        {
            _animator.SetBool(PARAM_IS_HOLDING, true);
        }

        _hand.Item.GameObject.layer = 2;
        _hand.ItemPosition = _hand.transform.position;
        _hand.ItemParent = _hand.transform;
        _detectRange.Detecteds.Clear();
    }

    
    private void DropItem(Vector2Int position)
    {
        _animator.SetLayerWeight(1, 0f);
        _animator.SetBool(PARAM_IS_HOLDING_TOOL, false);
        _animator.SetBool(PARAM_IS_HOLDING, false);

        if (_hand.Item != null)
        {
            // 레일을 배치할 수 있는 경우에 개수를 하나 줄이고 배치
            if (_hand.Item.BlockType == BlockType.Rail)
            {   
                if (RailManager.Instance.TryRailwayPlace(position))
                {
                    Rail rail = _hand.Item as Rail;
                    rail.ReduceStack();

                    if (rail.Count == 0)
                    {
                        _hand.Item = null;
                    }
                    return;
                }
            }
            ToolBase tool = _hand.Item as ToolBase;
            if (tool != null)
            {
                tool.ToolUI(true);
            }
            

            _hand.Item.GameObject.layer = 0;
            _hand.Item.GameObject.transform.rotation = Quaternion.identity;
            _hand.ItemPosition = position.CoordToWorld();
            _hand.ItemParent = null;
            _detectRange.Detecteds.Clear();

            // 배치했으니 로드된 청크에 저장
            Map.Instance.AddLoadedPoolable(_hand.Item as IPoolable);
        }

        Map.Instance.SetHoldable(position, _hand.Item);

        _hand.Item = null;
    }

    /// <summary>
    /// 선택된 IInteractable과 자동 상호작용
    /// </summary>
    public void TryAutoInteract(IInteractable target)
    {
        if (_hand.Item == null || target == null || _hand.Item == target)
        {
            _animator.SetBool(PARAM_IS_USING, false);
            return;
        }

        if((target is ResourceTree && _hand.Item is ToolAxe) || (target is ResourceRock && _hand.Item is ToolPickaxe))
        {
            _animator.SetBool(PARAM_IS_USING, true);
        }
        else
        {
            _animator.SetBool(PARAM_IS_USING, false);
        }

        target.AutoInteract(_hand.Item);
    }

    private void Init()
    {
        _moveSpeed = BASE_MOVE_SPEED;
        _canDash = true;
    }
    
}
