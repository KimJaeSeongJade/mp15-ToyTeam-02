using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefauliRails : MonoBehaviour
{
  [SerializeField] private RailManager _railManager;
  [SerializeField] private Rail _rail1;
  [SerializeField] private Rail _rail2;
  [SerializeField] private Rail _rail3;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      InitLinkedList();
    }
  }

  private void InitLinkedList()
  {
    _railManager.Rails.AddLast(_rail1);
    _railManager.Rails.AddLast(_rail2);
    _railManager.Rails.AddLast(_rail3);
    
  }
  
  
  
  
}
