# 열차 상호작용 및 레일 자동 생산 시스템

## 📝 1. 기능 개요

### 1.1. 기능 정의

- 열차의 화물칸(`CargoCart`) 및 제작칸(`CraftCart`)이 상호작용을 통해 플레이어와 재료를 주고 받는 기능

- 조건 충족 시 레일을 자동 제작하여 플레이어에게 공급하는 기능

- Spline 기반의 열차 주행 기반 클래스(`Train`)를 상속 

- 각 카트의 고유 상호작용 및 연출 상태(메쉬 갱신, 프로그래스 바 UI, 사운드) 처리

### 1.2. 구현 목표

- 플레이어가 수집한 재료(목재, 철)을 화물칸에 수납하면, 

  연결된 제작칸으로 이벤트를 전달하여 레일 제작 개시

- 손에 들고 있는 상태(빈 손, 레일 보유 중)에 맞추어 레일의 중첩 상태를 계산 후 전달

- 자원 수납량 및 제작 현황에 따른 열차칸 비주얼 동적 갱신 및 제작 진행률 UI/SFX 연동

### 1.3. 클래스 설명 

- `Train.cs`:

    - SplineAnimate를 활용한 열차 이동, 정지, 출발 제어 

    - 레일 연결 상태 이벤트를 수신하는 베이스 클래스

- `CargoCart.cs`:

    - 플레이어로부터 목재(`Wood`) 및 철(`Iron`)을 전달받아 최대 보관량만큼 저장

    - 연결된 `CraftCart`의 제작 로직(`TryCraft`)을 연쇄적으로 호출

- `CraftCart.cs`:

    - 화물칸의 자원(목재 1, 철 1)을 소모하여 일정 시간(`craftTime`) 동안 레일을 자동 제작

    - 제작 완료된 레일을 오브젝트 풀링(`ObjectPool`) 및 중첩(`AutoInteract`) 로직을 통해 플레이어에게 전달

---

## 🛠️ 2. 작성 클래스 설계 및 구조 

### 🔗 2.1. 클래스 간 관계도

<table border="0">
  <tr>
    <td width="40%" align="center" valign="top">
      <img src="./images/Train_Cart_ClassDiagrams.png" width="70%" alt="클래스 관계도" />
    </td>
    <td width="50%" valign="top">


## 핵심 클래스 세부 역할

### `Train.cs`

- 각 열차칸 `CargoCart.cs` / `CraftCart.cs`의 베이스 클래스

<br>

### `CargoCart.cs`

#### 1. 자원 수납 (`PushResource`) 

- 플레이어가 들고 있는 재료`(BlockType)`의 종류(목재/철)와 수량을 판별하여 수납 

- 남은 자원은 플레이어 손에 유지가능하도록 반환

#### 2. 제작 연동 (`TryStartTargetCraft`)

- 재료가 수납되면 연결된 `CraftCart`에 제작 가능 여부 판단을 요청

#### 3. 비주얼 갱신 (`UpdateResourceVisual`)

- 보관된 목재/철 수량(0~3개)에 따라 메쉬 프리팹을 동적으로 활성화

<br>

### `CraftCart.cs`

#### 자동 제작 루틴 (`CraftRoutine`) 

- 화물칸 자원을 차감(목재 1, 철 1) 후 타이머 기반 코루틴을 통해 레일 제작

- 코루틴이 적용된 누적 흐름에 따라 UI 프로그래스 바 및 입체적 SFX 재생

#### 아이템 전달 (`ButtonInteract`)

- 플레이어의 소지 여유 공간(`remainSpace`)을 계산하여 오브젝트 풀에서 레일을 꺼내 전달

- 플레이어 손에 든 레일에 중첩(`AutoInteract`)시켜 전달

    </td>
  </tr>
</table>

---

### 🔄 2.2. 실행 절차 흐름도 

<table border="0">
  <tr>
    <td width="40%" align="center" valign="top">
      <img src="./images/Train_Cart_InteractFlow.png" width="90%" alt="실행 절차 흐름도">
    </td>
    <td width="50%" valign="top">

## 단계별 세부 설명

<br>

### 1. 재료 수납 및 제작칸 전달 (`CargoCart`)

- 플레이어가 자원 아이템을 들고 `CargoCart`와 상호작용(`ButtonInteract`) 호출

- `PushResource()`를 통해 최대 보관 수량(`maxResourceCount`) 검증 후 자원 저장 및 비주얼 갱신

- 자원 수납 완료 즉시 연결된 `CraftCart.TryCraft()`를 호출하여 연쇄 제작 검사 진행

<br>
<br>

---

<br>

### 2. 레일 자동 제작 프로세스 (`CraftCart`)

- ### 1) 제작 조건 검증(중단 조건): 
    - 이미 제작 중(`isCrafting`) 

    - 제작칸 슬롯이 가득 찬 경우(`maxRailStorage`)
    
    - 재료가 부족 한 경우

    <br>

- ### 2) 자원 소모 & 제작 진행: 

    - `CargoCart.ConsumeResources(1, 1)`로 자원 차감 후 `CraftRoutine` 실행 

    - UI 게이지 및 망치/철/나무 SFX 재생)

    <br>

- ### 3) 제작 완료 & 연쇄 재시도
    - 보관 레일 수량 증가(`currentCraftCount++`) 후 남은 자원이 있다면 연쇄적으로 `TryCraft()` 재호출

    <br>
    <br>

    ---

    <br>

### 3. 제작 후 플레이어 전달 (`CraftCart`)

- ### 1) 잔여 공간 계산: 

    - 플레이어가 빈손일 경우 최대 수량으로 계산

    - 레일을 들고 있을 경우 소지 가능 여유 공간(`remainSpace`) 계산

    <br>

- ### 2) 풀링 및 중첩 전달:

  - 빈손: `ObjectPool.Take(BlockType.Rail)`로 생성하여 손으로 전달

  - 레일 보유 중: 부족한 개수만큼 임시 레일을 소환하고,
    
    `AutoInteract()`를 통해 기존 손의 레일에 중첩 합치기 수행

    </td>
  </tr>
</table>

---

## 🚨 3. 주요 이슈 및 디버깅 사례

### 3.1. 플레이어 소지량에 따른 레일 중첩 오작동 문제

#### 발생 문제

1. 플레이어가 이미 레일을 들고 있는 상태에서,
    
2. `CraftCart`의 완성된 레일 여러 개를 한 번에 수거할 때, 
    
3. 플레이어 손에 든 레일의 중첩 수량(`Count`)이 정확히 계산되지 앟는 현상 발생

#### 원인 분석

- 단일 오브젝트 생성 전달 방식으로는 기존 `Rail.cs` 내부에 구현된 

  중첩 로직(`AutoInteract`)을 거치지 않고 단순 수량만 바뀌어 데이터의 불일치가 발생함

#### 해결 방법

- `ButtonInteract` 내에서 전달할 수량(`amountToGive`)을 정밀 계산

- 부족한 개수만큼 `ObjectPool`에서 임시 레일(`tempRail`)을 동적으로 꺼낸후

   `tempRail.AutoInteract(handRail)`를 순회하도록해 중첩 기능 구현
   
---

### 3.2. 화물칸 자원 수납 시 잔여 아이템 처리 미비로 인한 아이템 잔류 이슈

#### 발생 문제

1. 플레이어가 소지한 자원을 `CargoCart`에 투입할 때,

2. 화물칸 공간이 충분하여 수납 조건(수량)을 통과했음에도 불구하고, 

3. 플레이어 손에서 아이템이 사라지지 않고 그대로 남아있는 현상 발생

#### 원인 분석

- `CargoCart.PushResource()`에서 자원을 모두 수납한 후 

  플레이어가 들고 있는 `MaterialBase` 객체를 오브젝트 풀로 반환(`ReturnToPool()`)하지 않거나, 

- 플레이어 손에 들린 데이터를 비우기 위한 `null` 반환 처리가 정상적으로 이루어지지 않아,

  플레이어가 손에 든 데이터가 유지되었음

#### 해결 방법

- `플레이어의 보유량`이 `화물칸의 남은 공간` 이하일 경우(= 소지한 재료를 모두 화물칸에 넣는 경우) 

  `material.ReturnToPool()`을 호출해 손에든 오브젝트를 풀에 반납하도록 강제

- `PushResource()`의 최종 반환값을 `null`로 지정, 

   플레이어 상호작용 로직에서 빈손 상태로 즉시 갱신되도록 수정