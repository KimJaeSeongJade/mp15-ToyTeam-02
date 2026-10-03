using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
