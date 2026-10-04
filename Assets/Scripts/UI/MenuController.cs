using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private KeyCode _upKey => KeyCode.UpArrow;
    private KeyCode _downKey => KeyCode.DownArrow;

    [SerializeField] private List<GameObject> _panel = new();
    private int _currenNum;

    private void OnEnable()
    {
        for(int i = 0; i<_panel.Count;i++)
        {
            _panel[i].transform.Find("Image").GetComponent<Image>().enabled = false;
        }
        _currenNum = 0;
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
    }

    private void OnDisable()
    {
        _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(_upKey))
        {
            Up();
        }
        if (Input.GetKeyDown(_downKey))
        {
            Down();
        }
    }

    private void Up()
    {
        if (_currenNum > 0)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            _currenNum--;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
        }
    }

    private void Down()
    {
        if (_currenNum < _panel.Count - 1)
        {
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = false;
            _currenNum++;
            _panel[_currenNum].transform.Find("Image").GetComponent<Image>().enabled = true;
        }
    }
}
