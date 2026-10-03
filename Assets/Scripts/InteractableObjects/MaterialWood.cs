using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWood : MaterialBase
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType => BlockType.Wood;
}
