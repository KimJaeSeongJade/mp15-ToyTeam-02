using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestTrain : MonoBehaviour
{
    [SerializeField] protected Train headCart; // 앞쪽 열차칸
    [SerializeField] protected Train tailCart; // 뒤쪽 열차칸
    
    [SerializeField] protected float followDistance = 1.0f; // 앞쪽 열차칸 사이의 간격
    [SerializeField] protected float moveSpeed = 3.0f;      // 이동속도


}
