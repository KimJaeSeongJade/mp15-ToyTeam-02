using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용 가능한 오브젝트
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 자신의 게임 오브젝트
    /// </summary>
    public GameObject GameObject { get; }

    /// <summary>
    /// 자신의 블록 종류
    /// </summary>
    public BlockType BlockType { get; }

    /// <summary>
    /// 플레이어가 raycast로 자동 상호작용
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable interactable);

    /// <summary>
    /// 플레이어가 버튼 눌러서 상호작용
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    /// <returns> 상호작용 이후 플레이어가 들어야 할 IInteractable </returns>
    public IInteractable ButtonInteract(IInteractable interactable);

    /// <summary>
    /// 자신의 외곽선 표시 활성화
    /// </summary>
    public void Targeted();

    /// <summary>
    /// 자신의 외곽선 표시 비활성화
    /// </summary>
    public void Untargeted();
}
