using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWood : Material
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType => BlockType.Wood;
}
