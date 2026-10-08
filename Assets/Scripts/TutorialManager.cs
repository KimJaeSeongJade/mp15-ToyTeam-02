using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] SplineManager _splineManager;

    [Header("UI Manager")]
    [SerializeField] private TutorialUIManager uiManager;
    [SerializeField] private FilledBarUI _filledBarUI;

    public static TutorialManager Instance;

    private int currentStep = 0; // 튜토리얼 진행 단계

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
            // [단계 0] 게임안내 1 팝업
            case 0:
                uiManager.HideAllHUDs();
                uiManager.OpenPopup(0);
                break;

            // [단계 1] 게임안내 2 팝업
            case 1:
                uiManager.OpenPopup(1);
                break;

            // [단계 2] 목재 채집 안내 팝업
            case 2:
                SpawnBlock(BlockType.Wood);
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
                SpawnBlock(BlockType.Rock);
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
                uiManager.OpenPopup(6); // 6번(마지막) 팝업: 튜토리얼 클리어 팝업
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
    /// 튜토리얼 나무 채집될 때 호출
    /// </summary>
    public void OnTreeMined()
    {
        Debug.Log("TreeMined");
        StartStep(4);
    }

    /// <summary>
    /// 튜토리얼 돌 채집될 때 호출
    /// </summary>
    public void OnRockMined()
    {
        StartStep(6);
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
    public void OnGameEnd()
    {
        if (currentStep == 9)
        {
            _splineManager.TrainDepart();
            StartStep(10); // 최종 클리어 팝업 호출
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