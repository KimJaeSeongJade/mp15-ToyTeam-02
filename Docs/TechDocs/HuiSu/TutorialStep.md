# 튜토리얼 시스템 및 단계별 진행 흐름

## 📝 1. 기능 개요

### 1.1. 기능 정의

- 플레이어가 게임의 핵심 루프(채집 ➔ 제작 ➔ 설치 ➔ 열차 주행 및 재설치)를 

   단계별로 습득할 수 있도록 가이드하는 튜토리얼 전용 상태 관리 시스템

- 팝업 UI(`TutorialUIManager`), 이벤트 트리거(`WoodTrigger`, `IronTrigger`, `CraftTrigger`) 및 

  싱글톤 관리자(`TutorialManager`)를 통해 튜토리얼을 단계별로 제어함

### 1.2. 구현 목표

- 단계별 팝업 창 활성화 시 `Time.timeScale = 0f` 및 

  캐릭터 제한(`RigidbodyConstraints`)를 통해 튜토리얼을 위한 환경 조성

- 플레이어의 특정 행동(나무 채집, 돌 채집, 레일 제작, 게임오버 발생)을 

  트리거 및 이벤트를 통해 감지하여 다음 튜토리얼 단계로전환

- 로딩 완료(`FilledBarUI`) 시점부터 최종 클리어 팝업 호출까지, 

  다단계(Step 0~13) 유한상태머신(Finite State Machine) 설계 적용

### 1.3. 클래스 설명 

- `TutorialManager.cs`:

    - 튜토리얼 전체 진행 단계(`currentStep`) 관리 및 게임 정지/재개(`PauseGame`/`ResumeGame`) 처리

    - 외부 트리거 및 UI 팝업 닫힘 이벤트를 수신하여 단계 전환 제어

- `TutorialUIManager.cs`:

    - 팝업 UI 목록(`popupList`) 및 가이드 HUD 목록(`hudList`)의 활성화/비활성화 제어

    - 엔터 키(`KeyCode.Return`) 입력을 감지하여 팝업을 닫고, 
    
      닫힌 인덱스를 이벤트를 통해 `TutorialManager`로 전달

- `CraftTrigger.cs` / `IronTrigger.cs` / `WoodTrigger.cs`:

    - 플레이어의 실습 행동(목재/철 채집, 레일 완성) 발생 시점을 감지하여 
    
      `TutorialManager`로 전달하는 보조 컴포넌트

---

## 🛠️ 2. 작성 클래스 설계 및 구조 

### 🔗 2.1. 클래스 간 관계도

<table border="0">
  <tr>
    <td width="40%" align="center" valign="top">
      <img src="./images/TutorialStep_ClassDiagrams.png" width="70%" alt="클래스 관계도" />
    </td>
    <td width="50%" valign="top">

## 핵심 클래스 세부 역할

### `TutorialManager.cs`

#### 단계별 흐름 제어 (`StartStep`)

- `currentStep` 인덱스에 따라 팝업을 열거나, 실습에 필요한 재료(`SpawnBlock`)을 오브젝트 풀에서 소환

#### 이벤트 수신 및 지연 처리 (`HandlePopupClosed`, `PopUpDelayRoutine`) 

- 채집 실습 성공 시 딜레이를 주어 다음 단계로 자연스럽게 이행

#### 인게임 상태 제어 (`PauseGame`, `ResumeGame`) 

- 팝업 노출 여부에 따라 `Time.timeScale`을 조절하여, 플레이어가 튜토리얼을 진행하기 위한 환경 조성

---

### `TutorialUIManager.cs`
#### 팝업 및 HUD 갱신 (`OpenPopup`, `ShowHUD`)

- 튜토리얼 단계와 매칭되는 인덱스의 UI만 활성화하고, 나머지는 모두 비활성화 처리하는 UI 오브젝트 관리


#### 캐릭터 오브젝트 통제 

- 팝업 오픈 시 `RigidbodyConstraints.FreezePosition`으로 캐릭터 이동 제한 
  
- 팝업이 닫힐 때 `FreezeRotation`으로 복구

---

### `Triggers (Wood/Iron/CraftTrigger)`
#### `WoodTrigger` / `IronTrigger`

- `OnEnable` 시점 및 재활성화 시점을 감지하여, `OnTreeMined()`, `OnRockMined()` 호출

#### `CraftTrigger`

- `CraftCart.CurrentCraftCount >= 1` 조건을 감지하여, 레일 제작 완료 이벤트(`OnRailCrafted()`) 발행

    </td>
  </tr>
</table>

---

### 🔄 2.2. 실행 절차 흐름도 

<table border="0">
  <tr>
    <td width="40%" align="center" valign="top">
      <img src="./images/TutorialStep_Flow.png" width="90%" alt="실행 절차 흐름도">
    </td>
    <td width="50%" valign="top">

## 단계별 세부 설명

### 1. 게임 안내 팝업 및 실습 재료 준비 (`Step 0 ~ 4`)
#### [Step 0~3] 기본 안내

- 게임 규칙, 클리어/오버 조건, 조작법 팝업을 순차적으로 노출 

- 엔터 키 입력 시 `HandlePopupClosed`를 통해 다음 단계 이동

- **[Step 4] 목재 실습 준비**: `ObjectPool`에서 도끼(`Axe`)와 나무(`Tree`)를 소환하고 목재 채집 안내 팝업 노출


---

### 2. 채집 및 제작 실습 단계 (`Step 5 ~ 9`)
### [Step 5~7] 목재 채집 & 철 실습

  - 팝업이 닫히면 `ResumeGame()`으로 게임 재개

  - 플레이어가 나무 파괴 시 `WoodTrigger` ➔ `OnTreeMined()` ➔ `PopUpDelayRoutine(6)` 코루틴 실행

  - 0.7초 후 게임을 일시정지하고 곡괭이(`Pickaxe`)와 돌(`Rock`)을 소환한 뒤 다음 팝업 호출

### [Step 8~9] 철 채집 & 레일 제작 실습

  - 돌 파괴 시 `IronTrigger` ➔ `OnRockMined()` 감지 후 레일 제작 안내 팝업 노출

  - 레일 제작 실습 시 `CraftTrigger`가 `CraftCart` 내 완성품 1개 이상을 감지 시 `OnRailCrafted()` 호출


---


### 3. 레일 설치, 열차 주행 및 최종 클리어 (`Step 10 ~ 13`)
#### [Step 10~11] 레일 설치 실습

  - 레일 설치 안내 팝업 후 게임 재개 (`ShowHUD(3)`)

  - 플레이어가 레일 설치 완료 후 열차 연결 이벤트 수신을 기다림

#### [Step 12~13] 열차 출발 및 최종 클리어

  - 게임오버/주행 이벤트(`OnGameEnd`) 감지 시 열차 출발(`TrainDepart()`) 및 재설치 안내 팝업 호출

  - 엔터 키 입력 시 최종 튜토리얼 클리어 팝업(`OpenPopup(9)`)을 노출하고 메인 메뉴 이동 활성화

    </td>
  </tr>
</table>

---

## 🚨 3. 주요 이슈 및 디버깅 사례

### 3.1. 팝업 노출 시 게임 내부 시간 정지로 인한 조작 미통제 문제

#### 발생 문제 

1. 튜토리얼 가이드 팝업이 켜졌을 때 `Time.timeScale = 0f`를 통해 게임 내부 시간만 정지시킨 결과, 

2. 유저의 입력(`Input`) 처리 및 물리 연동이 살아있어 팝업 창이 떠 있는 상태에서도 

3. 플레이어 캐릭터가 이동하거나 동작하는 현상 발생

#### 원인 분석 

- `Time.timeScale = 0f`는 프레임 기반의 시간 흐름(DeltaTime)만 멈출 뿐,

   유저 입력 감지의 예외처리가 없어 않아 캐릭터의 조작이 통제되지 않음

#### 💡 해결 방법** 

- `TutorialUIManager.OpenPopup()` 호출 시 

  `_playerRigidBody.constraints = RigidbodyConstraints.FreezePosition`을 적용
  (플레이어의 위치 이동을 강제로 고정)

- 팝업이 닫히는 시점(`ClosePopup`)에 `FreezeRotation`상태로 되돌려, 캐릭터의 행동을 제한

---

### 3.2. ⏱️ 실습 완료 직후 정지(Pause)로 인한 급박한 단계 전환 문제

### 발생 문제**

1. 플레이어가 나무나 돌을 파괴(채집)하는 실습을 완료하는 즉시 트리거가 즉각 발동

2. 곧바로 팝업이 열리고 게임이 정지되어 튜토리얼의 단계 전환이 부자연스럽게 느껴지는 현상 발생

### 원인 분석 

- 이벤트 감지 시점과 UI 팝업 오픈 사이에 딜레이가 없어, 다음 단계로 바로 넘어가짐

#### 해결 방법 

- `TutorialManager` 내에 `PopUpDelayRoutine` 코루틴을 작성

- 채집 완료 감지 시 `yield return delaySecond`(`WaitForSeconds(0.7f)`)의 지연 시간을 부여