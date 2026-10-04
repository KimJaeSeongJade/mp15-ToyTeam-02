using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IntroScene 관리 Manager
/// </summary>
public class IntroSceneManager : MonoBehaviour
{    
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManagerA.Instance.LoadTitleScene();
        }
    }
}
