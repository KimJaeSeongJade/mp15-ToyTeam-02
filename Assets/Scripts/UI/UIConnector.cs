using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnector : MonoBehaviour
{
    [SerializeField] private GameObject _childUI;
    public GameObject ChildUI
    {
        get 
        {  
            return _childUI; 
        }
    }

}