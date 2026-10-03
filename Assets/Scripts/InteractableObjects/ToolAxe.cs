using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAxe : ToolBase
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType { get  =>  BlockType.Axe; }
}
