using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftCart : Train, IInteractable
{
    [Header("화물칸 연결")]
    [SerializeField] private CargoCart targetCargoCart;    // 자원을 가져올 화물칸

    [Header("상태에 따른 제작칸 메쉬")]
    [SerializeField] private GameObject _railVisualTop;
    [SerializeField] private GameObject _railVisualMiddle;
    [SerializeField] private GameObject _railVisualBottom;

    [Header("제작 설정")]
    [SerializeField] private float craftTime = 3.0f;       // 레일 1개 제작 소요 시간
    [SerializeField] private int maxRailStorage = 3;       // 최대 레일 보관 개수 
    [SerializeField] private int currentCraftCount = 0;    // 현재 보관중인 레일 개수
    [SerializeField] private Outline _outline;
    [SerializeField] private Image _progressBar;
    [SerializeField] private Image _progressBarBackground;

    [SerializeField] private GameObject _progressBarUI;

    
    // 외부 참조용 프로퍼티
    public int CurrentCraftCount => currentCraftCount;
    public bool IsCrafting => isCrafting;

    public GameObject GameObject => gameObject;
    public BlockType BlockType => BlockType.None;

    private bool isCrafting = false;
    private AudioPlayer _rainHammerAudioPlayer;
    private AudioPlayer _rainIronAudioPlayer;
    private AudioPlayer _rainWoodAudioPlayer;

    protected override void OnEnable()
    {
        // 제작칸의 비주얼 상태 초기화
        UpdateCraftVisual();

        _rainHammerAudioPlayer = AudioManager.Instance.Take();
        _rainIronAudioPlayer = AudioManager.Instance.Take();
        _rainWoodAudioPlayer = AudioManager.Instance.Take();

        // 테두리 끄기
        if (_outline != null)
        {
            _outline.enabled = false;
            

        }
        
        if(_progressBarUI != null)
            _progressBarUI.SetActive(false);

        // 게임시작시 자원을 갖고 시작할 경우 제작 시도
        TryCraft();
        base.OnEnable();
    }

    /// <summary>
    /// 화물칸의 자원을 확인하고 제작 조건이 되면 제작 프로세스 시작
    /// </summary>
    public void TryCraft()
    {
        // 1. 이미 제작 중이면 진행 안 함
        if (isCrafting) return;

        // 2. 제작칸 보관함이 가득 차 있으면 진행 안 함
        if (currentCraftCount >= maxRailStorage) return;

        // 3. 연결된 화물칸이 없으면 진행 안 함
        if (targetCargoCart == null) return;

        // 4. 자원 부족 시 진행 안 함 
        if (targetCargoCart.CurrentWoodCount < 1 || targetCargoCart.CurrentIronCount < 1) return;

        // 조건을 만족하면 자원 소모 후 제작 코루틴 시작
        targetCargoCart.ConsumeResources(1, 1);
        StartCoroutine(CraftRoutine());
    }

    /// <summary>
    /// 제작 타이머 대기 후 레일 1개를 누적
    /// </summary>
    private IEnumerator CraftRoutine()
    {
        Debug.Log("CraftRoutine");

        // 제작 상태 시작 및 비주얼 갱신
        isCrafting = true;
        _rainHammerAudioPlayer
            .Init()
            .SetClip(_rainHammerClip)
            .SetPriority(50)
            .Play();


        _rainIronAudioPlayer
            .Init()
            .SetVolume(0.1f)
            .SetClip(_rainIronClip)
            .SetPriority(50)
            .Play();


        _rainWoodAudioPlayer
            .Init()
            .SetVolume(0.2f)
            .SetClip(_rainWoodClip)
            .SetPriority(50)
            .Play();

        _progressBar.fillAmount = 0;
        
        UpdateCraftVisual();
        _progressBarUI.SetActive(true);
        
        if (_progressBar != null)
        {
            float currentTime = 0f;

            while (currentTime < craftTime)
            {
                currentTime += Time.deltaTime;
                float currentProgress = currentTime / craftTime;
                _progressBar.fillAmount = currentProgress;
                _progressBarBackground.fillAmount = currentProgress;
                yield return  null;
            }
            currentTime = 0;
        }
        
        //yield return new WaitForSeconds(craftTime);

        // 레일 1개 생산 및 수량 제한 처리
        currentCraftCount++;
       _progressBarUI.SetActive(false);
        
        // 레일 최대 소지수 이상이면 제작 중단
        if (currentCraftCount > maxRailStorage)
        {
            currentCraftCount = maxRailStorage;
        }

        isCrafting = false;
        
        _rainHammerAudioPlayer
            .Stop();
        _rainIronAudioPlayer
            .Stop();
        _rainWoodAudioPlayer
            .Stop();
        
        UpdateCraftVisual();

        // 제작 완료 후 남은 자원 및 공간이 있다면 연쇄 제작 시도
        TryCraft();
    }

    /// <summary>
    /// 플레이어와 버튼으로 상호작용하여 제작 완료된 레일을 플레이어 손으로 전달합니다.
    /// </summary>
    /// <param name="inPlayerHand">플레이어가 손에 들고 있는 IInteractable 아이템</param>
    /// <returns>상호작용 후 플레이어가 손에 쥐게 될 IInteractable 레일</returns>
    public IInteractable ButtonInteract(IInteractable inPlayerHand)
    {
        // 제작칸에 완성된 레일이 없으면 그대로 반환
        if (currentCraftCount <= 0)
        {
            return inPlayerHand;
        }

        // 남은 공간(플레이어에게 넘겨주기 위한) 계산
        Rail handRail = inPlayerHand as Rail;
        int remainSpace = 0;

        // 1. 플레이어가 이미 레일을 들고 있는 경우
        if (handRail != null)
        {
            // 레일의 최대 소지량에서 현재 개수을 빼서 남은 공간 계산
            remainSpace = handRail.MaxStack - handRail.Count;
        }
        // 2.플레이어가 빈손인 경우(최대 소지량만큼 플레이어게 전달 가능)
        else
        {
            // 플레이어 손에 든 게 레일이 아닌 경우 반환
            if (inPlayerHand != null) return inPlayerHand;

            remainSpace = maxRailStorage;
        }

        // 3.플레이어의 손이 꽉찼다면 그대로 반환
        if (remainSpace <= 0)
        {
            return inPlayerHand;
        }

        // 실제 전달할 수량 계산
        
        int amountToGive = 0;
        
        // 1. 제작칸이 가진 레일수가 플레이어의 여유소지량 보다 많을 경우
        if (currentCraftCount >= remainSpace)
        {
            // 플레이어의 여유 소지량 만큼 넘겨줌
            amountToGive = remainSpace;
        }
        // 2. 제작칸에 남아있는 레일 수가 플레이어의 여유 소지량 보다 적은 경우:
        else
        {
            // 제작칸에 있는 레일 전부를 건네줍니다.
            amountToGive = currentCraftCount;
        }
        
        // 플레이어에게 레일으 전달하고 젲가칸의 레일 보유분은 감소
        currentCraftCount -= amountToGive;
        
        // 실제 전달할 레일 오브젝트 관리
        
        // 1. 플레이어가 빈 손이었던 경우
        if (handRail == null)
        {
            // 오브젝트풀에서 레일 1개를 꺼냄
            IPoolable poolable = ObjectPool.Instance.Take(BlockType.Rail);
            handRail = poolable as Rail;

            if (handRail != null)
            {
                handRail.gameObject.SetActive(true);
                
                // 레일 오브젝트 생성으로 이미 레일 클래스는 중첩수 1부터 시작
                // -> 추가 합칠 수량 1 차감
                amountToGive--;
            }
        }
        
        // 2. 플레이어가 빈 손이 아닌경우
        // 기존 Rail 클래스의 중첩 로직 활용 > AutoInteract() 메서드 활용
        // 나머지 전달 수량만큼 임시 레일을 소환하여 handRail에 합치기
        
        // 2개 이상을 한꺼번에 건네주는 경우, 부족한 개수만큼 루프를 돌며 처리
        for (int i = 0; i < amountToGive; i++)
        {
            // 중첩을 만들어 주기위한 임시 레일 생성
            IPoolable tempPoolable = ObjectPool.Instance.Take(BlockType.Rail);
            Rail tempRail = tempPoolable as Rail;

            if (tempRail != null)
            {
                tempRail.gameObject.SetActive(true);
                
                // 자동 상호작용 로직에 따라 생성된 tempRail 개수만큼 handRail에 중첩됨
                tempRail.AutoInteract(handRail);
            }
        }

        UpdateCraftVisual();

        // 제작 공간이 생겼으므로 추가 제작 시도
        TryCraft();

        // 완성된 레일 개체를 플레이어에게 전달
        return handRail;
    }

    /// <summary>
    /// 제작 진행 여부에 따라 제작칸 비주얼을 변경
    /// </summary>
    private void UpdateCraftVisual()
    {
        switch (CurrentCraftCount)
        {
            case 3:
                _railVisualTop.SetActive(true);
                _railVisualMiddle.SetActive(true);
                _railVisualBottom.SetActive(true);
                break;
            case 2:
                _railVisualTop.SetActive(false);
                _railVisualMiddle.SetActive(true);
                _railVisualBottom.SetActive(true);
                break;
            case 1:
                _railVisualTop.SetActive(false);
                _railVisualMiddle.SetActive(false);
                _railVisualBottom.SetActive(true);
                break;
            case 0:
                _railVisualTop.SetActive(false);
                _railVisualMiddle.SetActive(false);
                _railVisualBottom.SetActive(false);
                break;
        }
    }
    
    /// <summary>
    /// 플레이어가 접근하여 상호작용 가능한 경우 테두리 표시
    /// </summary>
    public void Targeted()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }
    
    /// <summary>
    /// 플레이어의 타게팅에서 벗어날 경우 테두리 비활성화
    /// </summary>
    public void Untargeted()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }

    /// <summary>
    /// 플레이어가 자동으로 상호작용 하는 경우
    /// </summary>
    /// <param name="interactable"> 플레이어가 손에 들고 있는 IInteractable </param>
    public void AutoInteract(IInteractable interactable)
    {

    }
}