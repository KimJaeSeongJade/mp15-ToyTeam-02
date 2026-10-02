using UnityEngine;

// 질문자님의 완벽한 뼈대인 'Resource'를 그대로 상속받습니다!
public class TestTree : Resource
{
    // 1. IPoolable 필수 요소 구현 (내 타입은 나무!)
    public override BlockType BlockType => BlockType.Tree;

    private void Awake()
    {
        DropMaterialType = BlockType.Wood;
        
    }
}