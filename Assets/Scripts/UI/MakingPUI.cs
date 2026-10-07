using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MakingPUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> _makingP;
    [SerializeField] private List<Vector3> _positionP = new ();

    private bool _isShowNow;

    private void Start()
    {
        Init();   
    }

    private void OnDisable()
    {
        if(_isShowNow == true)
        {
            HidePeople();
        }
    }

    public void Play()
    {
        ShowPeople();
    }

    private void ShowPeople()
    {
        if (!_isShowNow)
        {
            for (int i = 0; i < _makingP.Count; i++)
            {
                _makingP[i].SetActive(true);
                _isShowNow = true;
            }
        }
        else
        {
            HidePeople();
        }
    }

    private void HidePeople()
    {
        for (int i = 0; i < _makingP.Count; i++)
        {
            _makingP[i].GetComponent<Rigidbody>().transform.position = _positionP[i];
            _makingP[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
           _makingP[i].SetActive(false);
            _isShowNow = false;
        }
    }

    private void Init()
    {
        _isShowNow = false;
        for (int i =0; i< _makingP.Count; i++)
        {
            _positionP.Add(_makingP[i].transform.position);

            _makingP[i].SetActive(false);
        }
    }
}
