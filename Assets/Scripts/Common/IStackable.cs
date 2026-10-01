using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// 쌓을 수 있는 오브젝트
/// </summary>
public interface IStackable
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
    /// 쌓인 개수
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// 최대로 소지 가능한 개수
    /// </summary>
    public int MaxStack { get; }
}
