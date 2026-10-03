using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTest : ResourceTest
{
    public void AutoInteract(IInteractable interactable)
    {
        Debug.Log("autoInteract");
        AxeTest axe = interactable.GameObject.GetComponent<AxeTest>();
        Debug.Log(axe);
        if (axe == null) return;

        Destroy(this);
    }
}
