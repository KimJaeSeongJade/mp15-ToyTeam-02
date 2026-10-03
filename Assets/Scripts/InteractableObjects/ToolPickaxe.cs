using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPickaxe : Tool
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType { get => BlockType.Pickaxe; }
}
