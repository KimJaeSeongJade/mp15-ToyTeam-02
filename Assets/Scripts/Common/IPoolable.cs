using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀에서 꺼낼 수 있는 오브젝트
/// </summary>
public interface IPoolable
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
    /// 자기 자신을 오브젝트 풀로 반환
    /// </summary>
    public void ReturnToPool();
}
