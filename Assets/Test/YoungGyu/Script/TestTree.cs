using UnityEngine;

// 질문자님의 완벽한 뼈대인 'Resource'를 그대로 상속받습니다!
public class TestTree : Resource
{
    // 1. IPoolable 필수 요소 구현 (내 타입은 나무!)
    public override BlockType BlockType => BlockType.Tree;

    private void Awake()
    {
        // 2. 부서질 때 떨어뜨릴 재료를 '나무 장작'으로 고정
        _dropMaterialType = ObjectPoolManager.MaterialType.Wood;
        _resourceType = ObjectPoolManager.ResourceType.Tree;
    }

    private void Update()
    {
        // [테스트 1번] 나무 내구도 리셋 (다시 나타나기)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 부모(Resource)에 있는 Initialize()를 호출하면 내구도 3으로 꽉 차고 비주얼이 원래대로 돌아옵니다.
            Initialize();
            Debug.Log("1번 누름: 나무 상태가 완전히 초기화되었습니다! (내구도 3)");
        }

        // [테스트 2번] 채굴 (데미지 주기)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 부모(Resource)에 있는 ProcessMining에 1초(내구도 1칸)를 강제로 넘깁니다.
            ProcessMining(1.0f);
            Debug.Log($"2번 누름: 나무를 캤습니다! (남은 내구도: {Health})");
            
            // 주의: Health가 0이 되면 부모의 BreakResource()가 실행되면서 스스로 꺼지고 장작을 뱉습니다!
        }

        // [테스트 3번] 떨어진 장작(목재) 줍기
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 씬에 떨어져 있는 재료(Material)를 하나 찾습니다.
            Material droppedWood = FindObjectOfType<Material>();
            
            if (droppedWood != null && droppedWood.gameObject.activeInHierarchy)
            {
                // 플레이어가 먹은 것처럼 풀로 반납시켜 화면에서 없앱니다.
                droppedWood.ReturnToPool(droppedWood);
                Debug.Log("3번 누름: 떨어진 목재를 주워 먹었습니다!");
            }
            else
            {
                Debug.LogWarning("주울 목재가 없습니다! 먼저 2번을 눌러 나무를 부숴주세요.");
            }
        }
    }
}