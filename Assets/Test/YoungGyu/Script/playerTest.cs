using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTest : MonoBehaviour
{
    private IPoolable _tree;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _tree = ObjectPool.Instance.Take(BlockType.Tree);
            _tree.GameObject.SetActive(true);
            _tree.GameObject.transform.position = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _tree.GameObject.GetComponent<TestTree>().OnMined();
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            
        }
    }
    
}
