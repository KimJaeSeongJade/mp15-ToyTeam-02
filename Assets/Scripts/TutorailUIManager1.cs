using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorailUIManager : MonoBehaviour
{
    [Header("팝업 UI 목록")]
    [SerializeField] private List<GameObject> popupList;
    
    [Header("가이드 HUD 목록")]
    [SerializeField] private List<GameObject> hudList;
    
    [Header("씬 이름")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string mainStageSceneName = "MainStage";
    
    /// <summary>
    /// 팝업이 닫힐 때 TutorialManager에 알릴 이벤트
    /// </summary>
    public event Action<int> onPopupClosed;
    
    // 팝업 관리
    
    /// <summary>
    /// 지정한 번호 팝업만 켜고, 나머지 팝업은 끄기
    /// </summary>
    public void OpenPopup(int index)
    {
        for (int i = 0; i < popupList.Count; i++)
        {
            if (popupList[i] != null)
            {
                if (i == index)
                {
                    popupList[i].SetActive(true);  // 선택한 팝업만 켜기
                }
                else
                {
                    popupList[i].SetActive(false); // 나머지는 끄기
                }
            }
        }
    }
    
    /// <summary>
    /// 특정 번호의 팝업을 끄고, 닫혔다는 신호 알림
    /// </summary>
    public void ClosePopup(int index)
    {
        if (index >= 0 && index < popupList.Count && popupList[index] != null)
        {
            popupList[index].SetActive(false);

            // 지정한 팝업이 닫힌 이벤트를 TutorialManager에 전달
            onPopupClosed?.Invoke(index);
        }
    }
    
    //HUD 관리
    
    /// <summary>
    /// 화면에 떠 있는 모든 팝업을 끕니다.
    /// </summary>
    public void CloseAllPopups()
    {
        OpenPopup(-1);
    }
    /// <summary>
    /// 지정한 번호(index)의 HUD 1개만 켜고, 나머지는 무조건 끕니다.
    /// </summary>
    public void ShowHUD(int index)
    {
        for (int i = 0; i < hudList.Count; i++)
        {
            if (hudList[i] != null)
            {
                if (i == index)
                {
                    hudList[i].SetActive(true);  // 선택한 팝업만 켜기
                }
                else
                {
                    hudList[i].SetActive(false); // 나머지는 끄기
                }
            }
        }
    }

    /// <summary>
    /// 화면에 떠 있는 모든 HUD 끄기
    /// </summary>
    public void HideAllHUDs()
    {
        ShowHUD(-1);
    }

    /// <summary>
    /// 모든 팝업과 HUD를 한꺼번에 끄기
    /// </summary>
    public void CloseAllUI()
    {
        CloseAllPopups();
        HideAllHUDs();
    }
    
    // 튜토리얼 클리어 팝업 버튼 연동
    
    public void OnClickGoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnClickGoToMainStage()
    {
        SceneManager.LoadScene(mainStageSceneName);
    }
 
}
