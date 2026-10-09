using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TitleScene관리 매니저
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    private KeyCode _esc => KeyCode.Escape;
    [SerializeField] private GameObject _undoUI;
    [SerializeField] private GameObject _quitUI;
    [SerializeField] private GameObject _settingUI;  
    [SerializeField] private AudioClip _titleClip;
    [SerializeField] private AudioClip _birdClip;

    [SerializeField] private TextMeshProUGUI _highT;
    [SerializeField] private TextMeshProUGUI _highS;


    private PlayDataManager _pDataManager;

    private float _highTime;
    private int _highScore;

    //private bool _isSettingOpen;

    public Stack<(GameObject, GameObject)> StackUI = new ();

    private void Start()
    {
        _pDataManager = GetComponent<PlayDataManager>();
        FindHighScore();
        AudioPlayer titleAudioPlayer = AudioManager.Instance.Take();
        titleAudioPlayer
            .Init()
            .SetClip(_titleClip)
            .SetPriority(10)
            .Play();
        AudioPlayer titleBirdAudioPlayer = AudioManager.Instance.Take();
        titleBirdAudioPlayer
            .Init()
            .SetClip(_birdClip)
            .SetPriority(10)
            .Play();
    }
    
    private void Update()
    {  
        if(StackUI.Count == 0)
        {
            _quitUI.SetActive(true);
            _undoUI.SetActive(false);
        }
        else
        {
            _quitUI.SetActive(false);
            _undoUI.SetActive(true);
        }

        if(SceneManagerA.Instance._isPause && Input.GetKeyDown(_esc))
        {
            CloseSetting();
        }
    }

    public void FindHighScore()
    {
        GameMode gm = GameManager.Instance.GMode;
        GameDifficulty gd = GameManager.Instance.GDifficulty;
        List<PlayData> playDList = _pDataManager.LoadData().PlayDatas;

        for (int i = 0; i < playDList.Count; i++)
        {
            Debug.Log($"{playDList[i].GameMode} : {playDList[i].GameDifficulty} : {playDList[i].PlayTime} : {playDList[i].TrainDistance}");
        }

            if (playDList == null)
        {
            _highScore = 0;
            _highTime = 0;
        }

        else if(playDList != null)
        {
            _highScore = 0;
            _highTime = 0;
            for (int i =0;i<playDList.Count; i++)
            {                
                if (playDList[i].GameMode == gm && playDList[i].GameDifficulty == gd)
                {
                    if (playDList[i].TrainDistance > _highScore)
                    {
                        _highScore = playDList[i].TrainDistance;
                        _highTime = playDList[i].PlayTime;
                    }
                    else if (playDList[i].TrainDistance == _highScore)
                    {
                        if (playDList[i].PlayTime < _highTime)
                        {
                            _highScore = playDList[i].TrainDistance;
                            _highTime = playDList[i].PlayTime;
                        }
                        else if (playDList[i].PlayTime == _highTime)
                        {
                            continue;
                        }
                    }
                }
            }
        }
        RefreshText();
    }

    private void RefreshText()
    {
        _highT.GetComponent<TextNumChanger>().ChangeTimeNum(_highTime);
        _highS.GetComponent<TextNumChanger>().ChangeScoreNum(_highScore);
    }

    // UI뒤로가기
    public void Undo()
    {
        if(StackUI.Count < 1)
        {
            return;
        }

        else
        {
            (GameObject, GameObject) undoUI = StackUI.Pop();
            undoUI.Item1.SetActive(true);
            undoUI.Item2.SetActive(false);
        }
    }
    // Setting창 열기
    public void OppenSetting()
    {
        SceneManagerA.Instance._isSelectNow = false;
        SceneManagerA.Instance._isPause = true;
        SceneManagerA.Instance.Pause();
        _settingUI.SetActive(true);
        _settingUI.gameObject.GetComponent<MenuController>()._isSelect = true;
        _settingUI.gameObject.GetComponent<MenuController>()._isSettingUI = false;
    }
    // Setting창 닫기
    private void CloseSetting()
    {
        if (!SceneManagerA.Instance._isSelectNow)
        {
            SceneManagerA.Instance._isPause = false;
            SceneManagerA.Instance.Continue();
            _settingUI.SetActive(false);
            AudioPlayer titleAudioPlayer = AudioManager.Instance.Take();
            titleAudioPlayer
                .Stop();
            AudioPlayer titleBirdAudioPlayer = AudioManager.Instance.Take();
            titleBirdAudioPlayer
                .Stop();
        }
    }

    
}
