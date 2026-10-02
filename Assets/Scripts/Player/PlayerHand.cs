using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 손에 든 아이템 정보 저장
/// </summary>
public class PlayerHand : MonoBehaviour
{
    private IInteractable _item;

    /// <summary>
    /// 플레이어의 손에 든 아이템의 종류
    /// </summary>
    public IInteractable Item
    {
        get => _item;

        set
        {
            _item = value;
        }
    }

    /// <summary>
    /// 플레이어가 손에 든 아이템의 위치
    /// </summary>
    public Vector3 ItemPosition
    {
        get => _item.GameObject.transform.position;

        set
        {
            _item.GameObject.transform.position = value;
        }
    }

    /// <summary>
    /// 플레이어가 손에 든 아이템의 부모 오브젝트
    /// </summary>
    public Transform ItemParent
    {
        get => _item.GameObject.transform.parent;

        set
        {
            _item.GameObject.transform.parent = value;
        }
    }
}
