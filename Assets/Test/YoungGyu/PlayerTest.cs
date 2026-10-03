using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour,IInteractable
{
    [SerializeField] private GameObject _axe;

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
            IInteractable interactable = _tree.GameObject.GetComponent<IInteractable>();
            Debug.Log(_axe.GetComponent<IInteractable>());
            interactable.AutoInteract(_axe.GetComponent<IInteractable>());
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
        
    }

    public void Untargeted()
    {
        throw new System.NotImplementedException();
    }
}
