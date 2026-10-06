using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelListConnector : MonoBehaviour
{
    [SerializeField] private GameObject _otherList;
    public GameObject OtherList
    {
        get
        {
            return _otherList;
        }
    }

    [SerializeField] private List<GameObject> _pannelList = new();
    public List<GameObject> PannelList
    {
        get
        {
            return _pannelList;
        }
    }

    protected bool _isThis => false;

}