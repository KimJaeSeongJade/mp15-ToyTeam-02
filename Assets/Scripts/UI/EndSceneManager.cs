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
    
    [SerializeField] private AudioClip _gameOverClip;
    [SerializeField] private AudioClip _gameClearClip;

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
            AudioPlayer gameOver = AudioManager.Instance.Take();
            gameOver
                .Init()
                .SetVolume(1f)
                .SetClip(_gameClearClip)
                .SetLoop(false)
                .Play();

            _clearObj.SetActive(true);
            _overObj.SetActive(false);
        }
        else
        {
            
            //_clear.gameObject.SetActive(false);
            //_over.gameObject.SetActive(true);
            AudioPlayer gameclear = AudioManager.Instance.Take();
            gameclear
                .Init()
                .SetVolume(1f)
                .SetClip(_gameOverClip)
                .SetLoop(false)
                .Play();

            _clearObj.SetActive(false);
            _overObj.SetActive(true);
        }
    }

}
