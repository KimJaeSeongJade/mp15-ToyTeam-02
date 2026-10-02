using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : UIBase
{
    // Collider에 올라왔는지 확인하는 bool
    private bool _isOnTrigger;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.layer == 6)
        {
            _isOnTrigger = true;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.gameObject.layer == 6)
        {
            _isOnTrigger = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && _isOnTrigger)
        {
            PlayUI();
        }
    }

    public override void PlayUI()
    {
        Debug.Log("UI활성화");
    }
}
