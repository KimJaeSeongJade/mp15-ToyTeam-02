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
}
