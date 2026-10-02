using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron : MaterialBase
{
    public override BlockType BlockType => BlockType.Iron;
    
    
    
    private void OnEnable()
    {
        Initialize();
    }
    
    
}
