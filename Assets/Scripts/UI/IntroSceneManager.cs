using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IntroScene 관리 Manager
/// </summary>
public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private AudioClip _introClip;
    private AudioPlayer introPlayer;
    private bool _canInput;

    private void Start()
    {
        StartCoroutine(BlockInput());
        introPlayer = AudioManager.Instance.Take(true);
        introPlayer
            .Init()
            .SetClip(_introClip)
            .SetPriority(130)
            .Play();
    }

    private void Update()
    {        
        if (_canInput && Input.anyKeyDown)
        {
            SceneManagerA.Instance.LoadTitleScene();           
            introPlayer
                .Stop();            
        }
    }

    private IEnumerator BlockInput()
    {
        _canInput = false;
        yield return new WaitForSeconds(1.5f);
        _canInput = true;
    }
}
