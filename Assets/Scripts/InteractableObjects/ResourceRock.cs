using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRock : ResourceBase
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType => BlockType.Rock;
    public override BlockType ToolType => BlockType.Pickaxe;
    public override BlockType DropMaterialType => BlockType.Iron;
}