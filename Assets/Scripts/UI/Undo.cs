using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Undo : InteractUI
{
    [SerializeField] private UnityEvent _onUIPressed;

    public UnityEvent OnUIPressed => _onUIPressed;

    public override void PlayUI()
    {
        OnUIPressed.Invoke();
    }
}
