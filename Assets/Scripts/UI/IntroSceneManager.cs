using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IntroScene 관리 Manager
/// </summary>
public class IntroSceneManager : MonoBehaviour
{
    private bool _canInput;

    private void Start()
    {
        StartCoroutine(BlockInput());
    }

    private void Update()
    {        
        if (_canInput && Input.anyKeyDown)
        {
            SceneManagerA.Instance.LoadTitleScene();
        }
    }

    private IEnumerator BlockInput()
    {
        _canInput = false;
        yield return new WaitForSeconds(1.5f);
        _canInput = true;
    }
}
