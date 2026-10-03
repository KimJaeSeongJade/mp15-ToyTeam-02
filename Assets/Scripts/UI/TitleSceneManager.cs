using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TitleScene관리 매니저
/// </summary>
public class TitleSceneManager : MonoBehaviour
{    
    public Stack<(GameObject, GameObject)> StackUI = new ();

    // UI뒤로가기
    public void Undo()
    {
        if(StackUI.Count < 1)
        {
            Debug.Log("스택이 없습니다.");
        }

        else
        {
            (GameObject, GameObject) undoUI = StackUI.Pop();
            undoUI.Item1.SetActive(true);
            undoUI.Item2.SetActive(false);
        }
    }

    // 게임 종료
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Appleication.Quit();
        #endif
    }

}
