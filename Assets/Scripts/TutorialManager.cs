using UnityEngine;
using System.Collections;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] SplineManager _splineManager;

    [Header("UI Manager")]
    [SerializeField] private TutorialUIManager uiManager;
    [SerializeField] private FilledBarUI _filledBarUI;
    public static TutorialManager Instance;

    private int currentStep = 0; // 튜토리얼 진행 단계

    // 0.5초 대기용 코루틴 재사용을 위한 변수 선언
    private readonly WaitForSeconds delaySecond = new WaitForSeconds(0.7f);
    
    // 튜토리얼 씬에서만 존재하는 싱글톤
    private void Awake() => SetSingleton();
    // 1. 이벤트 연결 및 해제
    private void OnEnable()
    {
        BindGameClearEvents();

        // 튜토리얼로 게임 모드 설정 안 하면 GameMode가 Quick으로 변경
        GameManager.Instance.SetGameMode(GameMode.Tutorial);
        uiManager.CloseAllUI();

        // UIManager의 ClosePopup 연결(팝업이 닫힐 때 이벤트)
        if (uiManager != null)
        {
            uiManager.onPopupClosed += HandlePopupClosed;
            
        }
        _filledBarUI.OnBarLoaded += StartUIManager;
    }

    private void OnDisable()
    {
        UnbindGameClearEvents();

        // 이벤트 해제
        if (uiManager != null)
        {
            uiManager.onPopupClosed -= HandlePopupClosed;
           
        }
        _filledBarUI.OnBarLoaded -= StartUIManager;
    }

    private void StartUIManager()
    {
        // 로딩 완료 후 0번 단계(게임안내 1 팝업) 실행
        StartStep(0);
    }

    // 2, 튜토리얼 행동 단계별 UI상황 설정

    public void StartStep(int stepIndex)
    {
        currentStep = stepIndex;

        switch (currentStep)
        {
            // [단계 0] 팝업0: 게임안내 1 팝업
            case 0:
                uiManager.HideAllHUDs();
                uiManager.OpenPopup(0);
                
                break;

            // [단계 1] 팝업1: 게임안내 2 팝업
            case 1:
                uiManager.OpenPopup(1);
                break;
            
            // [단계 2] 팝업2: 클리어 및 게임 오버 안내 팝업
            case 2:
                uiManager.OpenPopup(2);
                break;
            
            // [단계 3] 팝업3: 조작방식 안내 팝업
            case 3:
                uiManager.OpenPopup(3);
                break;


            // [단계 4] 팝업4: 목재 채집 안내 팝업
            case 4:
                SpawnBlock(BlockType.Wood);
                uiManager.OpenPopup(4);
                break;

            // [단계 5] 팝업창에서 엔터 입력 -> 팝업 닫히고 게임 재개
            // 완료 조건은 플레이어가 목재 생성
            case 5:
                uiManager.CloseAllPopups();
                ResumeGame();
                uiManager.ShowHUD(0); 
                break;

            // [단계 6] 목재(BlockType.Wood) 생성 감지! -> 팝업5: 광석 채집 안내 팝업 + 게임 정지
            case 6:
                SpawnBlock(BlockType.Rock);
                PauseGame();
                uiManager.OpenPopup(5);
                
                break;

            // [단계 7] 엔터 입력 -> 팝업 닫힘 & 게임 재개 (철 생성 대기)
            case 7:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(1);
                ResumeGame();
                break;

            // [단계 8] 철(BlockType.Iron) 생성 감지! -> 팝업6: 레일 제작 안내 팝업 + 게임 정지
            case 8:
                PauseGame();
                uiManager.OpenPopup(6);
                break;

            // [단계 9] 엔터 입력 -> 팝업 닫힘 & 게임 재개 (레일 제작 실습)
            case 9:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(2);
                ResumeGame();
                break;

            // [단계 10] 레일 제작 완료 감지! -> 팝업7: 레일 설치 안내 팝업 + 게임 정지
            case 10:
                PauseGame();
                uiManager.OpenPopup(7); // 5번 팝업: 레일 설치 안내 팝업
                break;

            // [단계 11] 엔터 입력 -> 팝업 닫힘 & 게임 재개 (레일 설치 실습 및 기차 출발 대기)
            case 11:
                uiManager.CloseAllPopups();
                uiManager.ShowHUD(3);
                ResumeGame();
                break;
            
            // [단계 12] 게임매니저가 게임오버 상태를 만듬 -> 팝업8: 레일 재설치 안내 팝업 + 게임 정지
            case 12:
                PauseGame();
                uiManager.OpenPopup(8); // 5번 팝업: 레일 설치 안내 팝업
                break;

            // [단계 13] 엔터 입력 -> 튜토리얼 클리어 팝업 호출
            case 13:
                uiManager.OpenPopup(9); // 6번(마지막) 팝업: 튜토리얼 클리어 팝업
                break;
            // [단계 14] 엔터 입력 -> 튜토리얼 클리어 팝업 호출
            case 14:
                uiManager.OnClickGoToMainMenu(); // 메인 메뉴로 돌아가기
                break;
        }
    }

    
    // 다음 단계로 진행되는 조건(트리거) 모음

    /// <summary>
    /// UIManager에서 ESC 입력 으로 팝업이 닫힐 때호출
    /// </summary>
    private void HandlePopupClosed(int closedPopupIndex)
    {
        // [Step 0] 팝업0(게임안내1) 닫힘 -> Step 1(게임안내2) 진행
        if (currentStep == 0 && closedPopupIndex == 0)
        {
            StartStep(1);
        }
        // [Step 1] 팝업1(게임안내2) 닫힘 -> Step 2(클리어&오버 안내) 진행
        else if (currentStep == 1 && closedPopupIndex == 1)
        {
            StartStep(2);
        }
        // [Step 2] 팝업2(게임 클리어&오버 안내) 닫힘 -> Step 3(조작방식 안내) 진행
        else if (currentStep == 2 && closedPopupIndex == 2)
        {
            StartStep(3);
        }
        // [Step 3] 팝업3(조작방식 안내) 닫힘 -> Step 4(목재 채집 안내) 진행
        else if (currentStep == 3 && closedPopupIndex == 3)
        {
            StartStep(4);
        }
        // [Step 4] 팝업4(목재 채집 안내) 닫힘 -> Step 5(게임 재개 및 목재 채집 실습 시작) 진행
        else if (currentStep == 4 && closedPopupIndex == 4)
        {
            StartStep(5);
        }
        // [Step 6] 팝업5(광석 채집 안내) 닫힘 -> Step 7(게임 재개 및 철 채집 실습 시작) 진행
        else if (currentStep == 6 && closedPopupIndex == 5)
        {
            StartStep(7);
        }
        // [Step 8] 팝업6(레일 제작 안내) 닫힘 -> Step 9(게임 재개 및 레일 제작 실습 시작) 진행
        else if (currentStep == 8 && closedPopupIndex == 6)
        {
            StartStep(9);
        }
        // [Step 10] 팝업7(레일 설치 안내) 닫힘 -> Step 11(게임 재개 및 레일 설치치 실습 시작) 진행
        else if (currentStep == 10 && closedPopupIndex == 7)
        {
            StartStep(11);
        }
        // [Step 12] 팝업8(레일 재설치 안내) 닫힘 -> Step 13(최종 클리어 팝업 열기) 진행
        else if (currentStep == 12 && closedPopupIndex == 8)
        {
            StartStep(13);
        }
        // [Step 13] 팝업9(메인 메뉴로 돌아가기) 닫힘 -> Step 14(메인 메뉴로 돌아가기) 진행
        else if (currentStep == 13 && closedPopupIndex == 9)
        {
            StartStep(14);
        }
    }

    private void SpawnBlock(BlockType blockType)
    {
        if (blockType == BlockType.Wood)
        {
            IPoolable axe = ObjectPool.Instance.Take(BlockType.Axe);
            axe.GameObject.transform.position = new Vector3(13.5f, 0, 6.5f);
            axe.GameObject.SetActive(true);

            IPoolable tree = ObjectPool.Instance.Take(BlockType.Tree);
            tree.GameObject.transform.position = new Vector3(13.5f, 0, 9.5f);
            tree.GameObject.SetActive(true);
        }

        if (blockType == BlockType.Rock)
        {
            IPoolable pickaxe = ObjectPool.Instance.Take(BlockType.Pickaxe);
            pickaxe.GameObject.transform.position = new Vector3(17.5f, 0, 6.5f);
            pickaxe.GameObject.SetActive(true);

            IPoolable rock = ObjectPool.Instance.Take(BlockType.Rock);
            rock.GameObject.transform.position = new Vector3(17.5f, 0, 9.5f);
            rock.GameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 튜토리얼 실습 중 목재 채집될 때 호출
    /// </summary>
    public void OnTreeMined()
    {
        StartCoroutine(PopUpDelayRoutine(6));
    }

    /// <summary>
    /// 튜토리얼 실습 중 돌 채집될 때 호출
    /// </summary>
    public void OnRockMined()
    {
        StartCoroutine(PopUpDelayRoutine(8));
    }
    
    
    // 실습 완료후 0.5초 딜레이를 준뒤 팝업 호출
    private IEnumerator PopUpDelayRoutine(int nextStepIndex)
    {
        yield return delaySecond;
        
        StartStep(nextStepIndex);
    }
    
    

    /// <summary>
    /// 제작칸에서 부착된 추가 스크립트를 통해 제작칸의 레일 보유수 검사하고 호출 
    /// </summary>
    public void OnRailCrafted()
    {
        if (currentStep == 9)
        {
            
            StartStep(10);
        }
    }

    /// <summary>
    /// 메인 게임매니저에서 게임오버 상태를 만들때 호출
    /// </summary>
    public void OnGameEnd()
    {
        if (currentStep == 11)
        {
            _splineManager.TrainDepart();
            StartStep(12); // 레일 재설치 안내 팝업 호출
        }
    }
    
    // 팝업 닫고 켜질떄 시간 제어

    public void PauseGame() => Time.timeScale = 0f;
    public void ResumeGame() => Time.timeScale = 1f;

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void BindGameClearEvents()
    {
        GameSceneManager.Instance.OnGameEnd += OnGameEnd;
    }

    private void UnbindGameClearEvents()
    {
        GameSceneManager.Instance.OnGameEnd -= OnGameEnd;
    }
}