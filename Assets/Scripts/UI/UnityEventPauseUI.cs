using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventPauseUI : MonoBehaviour
{
    [SerializeField] private UnityEvent _onUIPressed;

    public bool _isSelect { get; private set; }
    public PMenuController PrevPMenu { get; private set; }
    public UnityEvent OnUIPressed => _onUIPressed;

    private void Update()
    {
        if (_isSelect)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {                
                SceneManagerA.Instance._isPause = false;
                OnUIPressed.Invoke();
            }
        }
    }

    public void SetPrevMenu(PMenuController prev)
    {
        PrevPMenu = prev;
    }

    public void SetSelect(bool value)
    {
        _isSelect = value;
    }

    private void CloseUI()
    {
        SceneManagerA.Instance._isSelectNow = false;

        PrevPMenu._isSelect = true;
        _isSelect = false;
        gameObject.SetActive(false);
    }
}
