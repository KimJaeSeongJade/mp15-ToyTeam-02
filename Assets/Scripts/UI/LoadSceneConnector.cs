using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneConnector : MonoBehaviour
{
    public void LoadTitleScene()
    {
        SceneManagerA.Instance.LoadTitleScene();
    }

    public void LoadGameScene()
    {
        SceneManagerA.Instance.LoadGameScene();
    }

    public void LoadTutorialScene()
    {
        SceneManagerA.Instance.LoadTutorialScene();
    }

    // 게임 종료
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
