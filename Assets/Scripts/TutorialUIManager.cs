using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUIManager : MonoBehaviour
{
    [Header("팝업 UI 목록")]
    [SerializeField] private List<GameObject> popupList;
    
    [Header("가이드 HUD 목록")]
    [SerializeField] private List<GameObject> hudList;
    
    [SerializeField] private Rigidbody _playerRigidBody;
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private PlayerController _playerController;

    
    /// <summary>
    /// 팝업이 닫힐 때 TutorialManager에 알릴 이벤트 (닫힌 팝업의 index 전달)
    /// </summary>
    public event Action<int> onPopupClosed;
    
    // 현재 화면에 실제로 열려 있는 팝업의 인덱스 (`-1`은 팝업이 아무것도 안열려 있는 상태)
    private int currentActivePopupIndex = -1;  

    /// <summary>
    /// 외부 참조용 프로퍼티 (현재 활성화된 팝업 번호 확인)
    /// </summary>
    public int CurrentActivePopupIndex => currentActivePopupIndex;
    
    // 1. 팝업 창 활성화 중 입력감지
    
    private void Update()
    {
        // 화면에 해당 단계의 팝업이 실제로 열려 있을때만 엔터 입력 받음
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentActivePopupIndex != -1 && currentActivePopupIndex != 10)
            {
                ClosePopup(currentActivePopupIndex);
            }
        }
    }

    
    // 2. 팝업 관리 메서드

    /// <summary>
    /// 지정한 번호의 팝업만 켜고, 현재 활성화된 팝업 번호 저장
    /// </summary>
    public void OpenPopup(int index)
    {
        _playerRigidBody.constraints = RigidbodyConstraints.FreezePosition;
        DisablePlayerControls();

        // 현재 활성화된 팝업 번호 저장
        currentActivePopupIndex = index;

        // 지정된 인덱스의 팝업만 활성화하고 나머지는 비활성화
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
    /// 특정 번호의 팝업을 닫기 + 팝업창 번호와 함께 팝업창 닫힘 이벤트를 전달
    /// </summary>
    public void ClosePopup(int index)
    {
        _playerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        EnablePlayerControls();

        // 인덱스 마이너스 일 때(즉 열린 팝업 리스트가 없을 떄),
        // 리스트 범위 초과할 때, 리스트에 팝업이 저장안되어 있으면 예외처리
        if (index >= 0 && index < popupList.Count && popupList[index] != null)
        {
            // 실제 해당 팝업 비활성화
            popupList[index].SetActive(false);

            // 현재 열려있던 팝업을 정상적으로 닫은 경우 상태 초기화
            if (currentActivePopupIndex == index)
            {
                currentActivePopupIndex = -1;
            }

            // 인덱스 번호를 담아서 팝업 닥힘 이벤트를 튜토리얼 매니저에 전달
            onPopupClosed?.Invoke(index);
        }
    }

    /// <summary>
    /// 화면의 모든 팝업을 비활성화 하기 (인게임 실습 모드 진입 시 호출)
    /// </summary>
    public void CloseAllPopups()
    {
        currentActivePopupIndex = -1; // 활성 팝업 없음 상태로 초기화
        
        for (int i = 0; i < popupList.Count; i++)
        {
            if (popupList[i] != null)
            {
                popupList[i].SetActive(false);
            }
        }
    }

    
    // 3. HUD 관리

    /// <summary>
    /// 지정한 번호(index)의 HUD 1개만 켜고, 나머지는 모두 비활성화
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
    /// 모든 HUD 비활성화
    /// </summary>
    public void HideAllHUDs()
    {
        ShowHUD(-1);   
    }

    /// <summary>
    /// 모든 팝업과 HUD를 한꺼번에 비활성화
    /// 최종 플레이어 실습끝나고 난 뒤 (튜토리얼 클리어 팝업으로 넘어가기전 까지) 호출
    /// </summary>
    public void CloseAllUI()
    {
        CloseAllPopups();
        HideAllHUDs();
    }
    
    /// <summary>
    /// 튜토리얼 클리어 팝어의 버튼 연동
    /// 메인 타이틀로 돌아가기
    /// </summary>
    public void OnClickGoToMainMenu()
    {
        // SceneManagerA.Instance.LoadTitleScene();
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    private void EnablePlayerControls()
    {
        _playerAction.enabled = true;
        _playerController.enabled = true;
    }

    private void DisablePlayerControls()
    {
        _playerAction.enabled = false;
        _playerController.enabled = false;
    }
}