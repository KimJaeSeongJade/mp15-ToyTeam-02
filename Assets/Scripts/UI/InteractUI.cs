using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : UIBase
{
    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            PlayUI();
        }
    }

    public override void PlayUI()
    {
        Debug.Log("UI활성화");
    }
}
