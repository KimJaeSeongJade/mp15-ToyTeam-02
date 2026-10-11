# Trigger UI

## 📝 1. 기능 개요

### 1.1. 기능 정의

- UIPlayer의 조작에 따라 Collider를 감지하여 작동하는 UI

### 1.2. 구현 목표

- 기본적으로 Collider를 감지하여 각각 2개의 Image와 Text를 스위칭
- UIPlayer와 상호작용 방법에 따라 각각의 UI기능 작동

### 1.3. 클래스 설명 

- `UIBase`:
    - 추상 클래스로 2개의 이미지와 2개의Text, Box Collider 컴포넌트로 구성되어 있음
    - Box Collider 감지에 따라 OnImage와 OffImage를 켜고 끔
    - 추상 메서드 PlayUI()를 정의하고 있음

- `InteractUI`:
    - UIBase를 상속받는 UI
    - 상화작용키 입력시 작동하는 UI
    - 기본적인 선택이나 작동이 필요한 UI에 사용

- `TimerUI`:
    - UIBase를 상속받는 UI
    - UI위에 위치한 시간에따라 게이지가 차오르며 다차오를시 작동하는 UI
    - 주로 게임씬으로 전환하는 UI들에 사용

- `LRUI`:
    - UIBase와 2개의 InteractUI로 이루어진 UI
    - UIBase를 중심으로 양옆에 InteractUI를 배치
    - 양옆 화살표 InteractUI를 작동시킴에 따라 중앙 Text가 변
    - 선택지를 고를 필요가 있는 UI에 사용

## 🛠️ 2. 작성 클래스 설계 및 구조 

### `UIBase`
- TriggerUI들의 베이스 추상 클래스
- OnTrigger Enter, Exit으로 [OnImage, Ontext]와 [OffImage, OffText]를 켜고 끔
- protectd abstract void PlayUI(); 추상 메서드 보유

### `InteractUI`
- 주로 UnityEventInteractUI로 변형해 사용하며 UnityEvent에 등록된 기능을 호출

### `TimerUI`
- OnTriggerStay로 CurrentTime()을 계산해 정해진 시간이 지나면 PlayUI()를 활성화
- 주로 UnityEventTimerUI로 변형해 사용하며 UnityEvent에 등록된 기능을 호출

### `LRUI`
- public virtual void ChangeNum(int num) 메서드를 이용해 페이지를 좌우로 넘김
- 주로 LR"해당기능"UI로 변형해 사용하며 변경된 값을 Singleton Manager들에게 전달

### 🔗 2.1. 클래스 간 관계도
<img src="./images/TriggerUI관계도.png">

### 🔄 2.2. 실행 절차 흐름
1. UI의 Collider가 UIPlayer를 감지
2. ON / Off Image전환
3. Trigger UI 종류에 따라 상호작용 또는 특정시간 대기
4. PlayUI() 메서드 동작

## 🚨 3. 주요 이슈 및 디버깅 사례

### 이미지 On Off 문제
#### 발생문제
특정 UI의 Text와 Image가 사라지는 현상 발생

#### 원인 분석
`Image[] images = GetComponentsInChildren<Image>()`
로 이미지를 찾아 구분하기 때문에
On / Off를 조작할 필요가 없는 Text를 표시만 하는 변형 UI들은 Image와 Text를 1개씩만 가지고 있어
On / Off 이미지 등록에 문제 발생

#### 해결 방법
if문으로 Image가 2개일 찾았을 경우와 아닐경우를 나눠 오류 발생 대비
및 Image가 1개인 UI는 PanelUI로 독립