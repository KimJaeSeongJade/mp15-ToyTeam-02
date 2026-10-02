using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MaterialBase
{
    public override BlockType BlockType => BlockType.Wood;



    private void OnEnable()
    {
        Initialize();
    }
    
}
