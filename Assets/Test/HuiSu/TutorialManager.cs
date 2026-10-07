using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Manager")]
    [SerializeField] private TutorialUIManager uiManager;

    private int currentStep = 0; // 튜토리얼 진행 단계
    
    public static event Action<BlockType> OnResourceDropped;

    public static void CallOnResourceDropped(BlockType type)
    {
        OnResourceDropped?.Invoke(type);
    }
    

    // 1. 이벤트 연결 및 해제
    
    private void OnEnable()
    {
        // UIManager의 ClosePopup 연결(팝업이 닫힐 때 이벤트)
        if (uiManager != null)
        {
            uiManager.onPopupClosed += HandlePopupClosed;
            
        }
        OnResourceDropped += OnBlockSpawned;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (uiManager != null)
        {
            uiManager.onPopupClosed -= HandlePopupClosed;
           
        }
        OnResourceDropped -= OnBlockSpawned;
    }

    private void Start()
    {
        // 씬 시작과 동시에 0번 단계(게임안내 1 팝업) 실행
        uiManager.CloseAllUI();
        StartStep(0);

    }


    
    // 2, 튜토리얼 행동 단계별 UI상황 설정

    public void StartStep(int stepIndex)
    {
        currentStep = stepIndex;

        switch (currentStep)
        {
            // [단계 0] 게임안내 1 팝업 + 게임 정지
            case 0:
                PauseGame();
                uiManager.HideAllHUDs();
                uiManager.OpenPopup(0);
                break;

            // [단계 1] 게임안내 2 팝업 + 게임 정지
            case 1:
                PauseGame();
                uiManager.OpenPopup(1);
                break;

            // [단계 2] 목재 채집 안내 팝업 + 게임 정지
            case 2:
                PauseGame();
                uiManager.OpenPopup(2);
                break;

            // [단계 3] 팝업창에서 ESC 입력 -> 팝업 닫히고 게임 재개
            // 완료 조건은 플레이어가 목재 생성
            case 3:
                uiManager.CloseAllPopups();
                ResumeGame();
                uiManager.ShowHUD(0); 
                break;

            // [단계 4] 목재(BlockType.Wood) 생성 감지! -> 광석 채집 안내 팝업 + 게임 정지
            case 4:
                PauseGame();
                uiManager.OpenPopup(3);
                
                break;

            // [단계 5] ESC 입력 ➔ 팝업 닫힘 & 게임 재개 (철 생성 대기)
            case 5:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(1);
                ResumeGame();
                break;

            // [단계 6] 철(BlockType.Iron) 생성 감지! -> 레일 제작 안내 팝업 + 게임 정지
            case 6:
                PauseGame();
                uiManager.OpenPopup(4);
                break;

            // [단계 7] ESC 입력 ➔ 팝업 닫힘 & 게임 재개 (레일 제작 실습)
            case 7:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(2);
                ResumeGame();
                break;

            // [단계 8] 레일 제작 완료 감지! -> 레일 설치 안내 팝업 + 게임 정지
            case 8:
                PauseGame();
                uiManager.OpenPopup(5); // 5번 팝업: 레일 설치 안내 팝업
                break;

            // [단계 9] ESC 입력 -> 팝업 닫힘 & 게임 재개 (레일 설치 실습 및 기차 출발 대기)
            case 9:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(3);
                ResumeGame();
                break;

            // [단계 10] TODO: 게임매니저가 게임오버 상태를 만듬
            case 10:
                PauseGame();
                uiManager.OpenPopup(6); // 6번(마지막) 팝업: 튜토리얼 클리어 팝업
                Debug.Log("[TutorialManager] 튜토리얼 전체 클리어!");
                break;
        }
    }

    
    // 다음 단계로 진행되는 조건(트리거) 모음

    /// <summary>
    /// UIManager에서 ESC 입력 으로 팝업이 닫힐 때호출
    /// </summary>
    private void HandlePopupClosed(int closedPopupIndex)
    {
        if (currentStep == 0 && closedPopupIndex == 0) StartStep(1);    // 게임안내 팝업2 열린상태 & 게임안내 팝업1 닫힌 상태(게임정지)
        
        else if (currentStep == 1 && closedPopupIndex == 1) StartStep(2);   // 목재 채집 팝업 열린상태(게임정지)
        
        else if (currentStep == 2 && closedPopupIndex == 2) StartStep(3);   // 플레이어 목재 채집 실습 시작
                                                                            // 목재 생성 감지트리거 -> 철 재집  팝업 열린상태(게임정지)
        
        else if (currentStep == 4 && closedPopupIndex == 3) StartStep(5);   // 플레이어 광석 채집 실습 시작
                                                                            // 철 생성 감지트리거 -> 레일 제작 팝업 열린상태(게임정지)
        
        else if (currentStep == 6 && closedPopupIndex == 4) StartStep(7);   // 레일 제작 실습 시작
        
        else if (currentStep == 8 && closedPopupIndex == 5) StartStep(9);   // 레일 설치 실습 시작
    }

    /// <summary>
    /// SpawnTrigger에서 자원 드롭 시 호출
    /// TODO: 플레이어가 자원을 손에들 때로 트리거 변경 필요
    /// </summary>
    public void OnBlockSpawned(BlockType type)
    {
        
        
        if (currentStep == 3 && type == BlockType.Wood)
        {
            StartStep(4);
            Debug.Log(" 목재 나옴!!");
            IPoolable pickaxe = ObjectPool.Instance.Take(BlockType.Pickaxe);
            
            pickaxe.GameObject.transform.position = new Vector3(17, 0, 6);
            pickaxe.GameObject.SetActive(true);
            
            IPoolable rock = ObjectPool.Instance.Take(BlockType.Rock);
            
            rock.GameObject.transform.position = new Vector3(17, 0, 9);
            rock.GameObject.SetActive(true);

            
        }
        else if (currentStep == 5 && type == BlockType.Iron)
        {
            StartStep(6);
            Debug.Log(" 철 나옴!!");
        }
    }

    /// <summary>
    /// 제작칸에서 부착된 추가 스크립트를 통해 제작칸의 레일 보유수 검사하고 호출 
    /// </summary>
    public void OnRailCrafted()
    {
        if (currentStep == 7)
        {
            
            StartStep(8);
        }
    }

    /// <summary>
    /// TODO: 메인 게임매니저에서 게임오버 상태를 만들때 호출
    /// </summary>
    public void OnTrainMoved()
    {
        
        if (currentStep == 9)
        {
            // TODO: 게임 매니저의 게임오버 상태 판정 가져오기
            Debug.Log("튜토리얼 클리어 팝업 조건 달성");
            StartStep(10); // 최종 클리어 팝업 호출
        }
    }
    
    // 팝업 닫고 켜질떄 시간 제어

    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;
}