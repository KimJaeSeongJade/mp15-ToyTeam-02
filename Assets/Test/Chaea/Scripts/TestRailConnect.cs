using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRailConnect : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector2Int coord = new Vector2Int(2, 9);
            IPoolable poolable = ObjectPool.Instance.Take(BlockType.Rail);
            Rail newRail = poolable as Rail;

            newRail.GameObject.transform.position = coord.CoordToWorld();
            newRail.GameObject.SetActive(true);

            Map.Instance.SetHoldable(coord, newRail);
        }
    }

    // TODO: 게임 클리어 후 열차가 파괴되지 않고 멈추되도록
    // TODO: railway를 놔도 열차 사이 간격이 벌어지지 않고 유지되도록
}
