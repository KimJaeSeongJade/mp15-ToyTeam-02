using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTreeTutorial : ResourceBaseTutorial
{
    [SerializeField] Outline _outline;

    public override Outline Outline => _outline;
    public override BlockType BlockType => BlockType.Tree;
    public override BlockType ToolType => BlockType.Axe;
    public override BlockType DropMaterialType => BlockType.Wood;
}