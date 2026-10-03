using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    //private Stack<PanelChange> _stackUI;
    public Stack<PanelChange> StackUI;
    //{
    //    get
    //    {
    //        return _stackUI;
    //    }
    //    set
    //    {
    //        _stackUI = value;
    //    }
    //}

    public void Undo()
    {
        PanelChange undoUI = StackUI.Pop();
        undoUI.Undo();
    }
}
