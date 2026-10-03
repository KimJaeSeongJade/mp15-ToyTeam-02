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
            Map.Instance.SetHoldable(interactable.transform.position.WorldToCoord(), interactable.GetComponent<IInteractable>());
            interactable.transform.position = interactable.transform.position.WorldToCoord().CoordToWorld();
        }

        foreach(GameObject interactable in interactables)
        {
            Debug.Log($"{Map.Instance.GetHoldable(interactable.transform.position.WorldToCoord())}");
        }
    }
}
