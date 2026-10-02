using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [SerializeField] GameObject[] interactables;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) Foo();
    }

    private void Foo()
    {
        foreach(GameObject interactable in interactables)
        {
            Map.Instance.SetInteractable(interactable.transform.position.WorldToCoord(), interactable.GetComponent<IInteractable>());
        }

        foreach(GameObject interactable in interactables)
        {
            Debug.Log($"{Map.Instance.GetInteractable(interactable.transform.position.WorldToCoord())}");
        }
    }
}
