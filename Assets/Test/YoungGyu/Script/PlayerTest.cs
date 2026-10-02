using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour,IInteractable
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
            _tree.GameObject.GetComponent<Tree>().OnMined();
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            
        }
    }

    public GameObject GameObject { get; }
    public BlockType BlockType { get; }
    public void AutoInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public IInteractable ButtonInteract(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }

    public void Targeted()
    {
        Debug.Log("PlayerTest : Targeted");
        _tree.GameObject.GetComponent<Tree>().OnMined();
        _tree.GameObject.GetComponent<Rock>().OnMined();
        
    }

    public void Untargeted()
    {
        throw new System.NotImplementedException();
    }
}
