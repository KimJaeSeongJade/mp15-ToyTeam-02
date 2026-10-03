using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialIron : Material
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType => BlockType.Iron;
}
