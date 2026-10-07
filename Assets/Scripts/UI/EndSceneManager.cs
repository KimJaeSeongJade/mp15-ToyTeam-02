using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI _clear;
    //[SerializeField] private TextMeshProUGUI _over;
    [SerializeField] private GameObject _clearObj;
    [SerializeField] private GameObject _overObj;

    private bool _isGameWin;

    private void Awake()
    {
        Init();   
    }

    private void Start()
    {
        SetGameEndScene();
    }

    private void Init()
    {
        _isGameWin = GameManager.Instance.IsGameWin;
    }


    private void SetGameEndScene()
    {
        if(_isGameWin)
        {
            //_clear.gameObject.SetActive(true);
            //_over.gameObject.SetActive(false);

            _clearObj.SetActive(true);
            _overObj.SetActive(false);
        }
        else
        {
            //_clear.gameObject.SetActive(false);
            //_over.gameObject.SetActive(true);

            _clearObj.SetActive(false);
            _overObj.SetActive(true);
        }
    }

}
