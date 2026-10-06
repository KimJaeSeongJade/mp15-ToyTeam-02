using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneConnector : MonoBehaviour
{
    public void LoadTitleScene()
    {
        SceneManagerA.Instance.LoadTitleScene();
    }
        public void LoadTutorialScene()
    {
        SceneManagerA.Instance.LoadTutorialScene();
    }

    public void LoadEndScene()
    {
        SceneManagerA.Instance.LoadEndScene();
    }

    public void LoadGameScene()
    {
        SceneManagerA.Instance.LoadGameScene();
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
