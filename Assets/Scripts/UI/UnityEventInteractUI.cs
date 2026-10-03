using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 상호작용시 버튼에 등록된 메서드를 작동
/// </summary>
public class UnityEventInteractUI : InteractUI
{
    [SerializeField] private UnityEvent _onUIPressed;

    public UnityEvent OnUIPressed => _onUIPressed;

    protected override void PlayUI()
    {
        OnUIPressed.Invoke();
    }
}
