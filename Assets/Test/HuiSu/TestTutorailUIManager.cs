using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTutorailUIManager : MonoBehaviour
{
    [Header("팝업 UI 목록")]
    [SerializeField] private List<GameObject> popupList;
    
    [Header("가이드 HUD 목록")]
    [SerializeField] private List<GameObject> hudList;
    
    [Header("씬 이름")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string mainStageSceneName = "MainStage";
    
    [Header("테스트용 ")]
    [SerializeField] private bool enableTestKeys = true;
    
    /// <summary>
    /// 팝업이 닫힐 때 TutorialManager에 알릴 이벤트
    /// </summary>
    public event Action<int> onPopupClosed;
    
    // 현재 열려 있는 팝업 번호를 기억하는 테스트용 변수
    private int testCurrentPopupIndex = -1;
    
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
    
    private void Update()
    {
        if (!enableTestKeys) return;

        // 키보드 숫자키 1 ~ 8 누를 때: 순서대로 팝업 호출
        for (int i = 0; i < popupList.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                OpenPopup(i);
            }
        }

        // 키보드 F1 ~ F4 키 누를 때: 순서대로 HUD 호출
        for (int i = 0; i < hudList.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.F1 + i))
            {
                ShowHUD(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 다음 팝업 번호로 1 증가
            testCurrentPopupIndex++;

            // 만약 팝업 개수(8개)를 초과하면(7번 다음인 8이 되면), 다시 처음(0번)으로 돌리거나 다 닫습니다.
            if (testCurrentPopupIndex >= popupList.Count)
            {
                testCurrentPopupIndex = -1; // -1로 초기화하여 모든 팝업 닫기
                CloseAllPopups();
                Debug.Log("[테스트] 마지막 팝업입니다");
            }
            else
            {
                OpenPopup(testCurrentPopupIndex);
                Debug.Log($"[테스트] ESC키 눌러 다음으로 진행");
            }
        }
    }
}
