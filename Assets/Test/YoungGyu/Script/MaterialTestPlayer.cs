using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTestPlayer : MonoBehaviour
{
    [SerializeField] private Tree _tree;
    [SerializeField] private Axe _axe;
    
    private void Intteract()
    {
        Debug.Log("MaterialTestPlayer : Intteract");
        Debug.Log(_axe);
        _tree.AutoInteract(_axe.GetComponent<IInteractable>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Intteract();
           
        }
    }

}
