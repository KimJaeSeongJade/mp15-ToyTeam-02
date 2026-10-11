# Input System

## 📝 1. 기능 개요

### 1.1. 기능 정의

- 마우스를 사용하지 않는 오직 키보드로만 조작 가능한 UI조작 System

### 1.2. 구현 목표

- 키보드만으로 UI를 조작
- WASD키로 UI를 선택 및 값을 조절

### 1.3. 클래스 설명

- `UIPlayerController`:
    - UIPlayerController는 기본 PlayerController에서 이동관련 기능만 가져온 Controller  
    - Title, End Scene에서 UIPlayer를 조작해 Trigger UI의 Trigger를 작동

- `MenuController`:
    - TitleScene의 설정UI 활성화시 조작을 담당하는 Controller
    - UIConnector에 연결된 UI의 MenuController와 연결을 주고 받으며 Bool로 현재 상황에 따른 UI와 조작키를 구분해 활성화

- `PMenuController (PauseMenuController)`:
    - Game/ Tutorial Scene의 Pause진입시 활성화 된 UI 조작을 담당하는 Controller  
    - MenuController와 구성은 비슷하지만 if문으로 PMenuController뿐 아니라 UnityEventPauseUI를 구분해 조작을 활성화

## 🛠️ 2. 작성 클래스 설계 및 구조 

### `UIPlayerController`
- PlayerController의 이동기능만 탑재한 Controller
- WASD 키로 UIPlayer를 이동
- TriggerUI 종류에 따라 UI기능 활성화

### `MenuController`
- MenuController는 좌 우 한 쌍으로 화면에 켜짐
- isSelect Bool은 W/S 키를 누르면 두 UI가 같이 작동하는 것을 막아 현재 조작할 UI를 확인하는 변수
- isSelect는 조작할 UI만 True 조작안하는 다른UI는 False로, 서로 True/False를 동작중에 바꿔줌
- isSettingUI는 **D키**의 역할이 왼쪽 MController에서는 오른쪽 UI로 진입하는 역할,
 오른쪽 UI에서는 값을 조절하는 키로 동작해야 때문에 값을 조절하는 UI인지를 체크하는 Bool변수
- SliderUI/LRUI를 담은 MenuController의 isSettingUI는 항상 True Panel을 담고있는 MController의 UI는 항상 Flase
- MenuController PrevM은 UIConnector로 오른쪽 MController로 진입한 뒤 다시 왼쪽UI로 돌아갈 때를 위한 변수

### `PMenuController`
- MenuController를 변형한 Controller
- MenuController에 담기지 않는 Space/ Esc만 입력받는 UnityEventPauseUI를 담아야함
- MenuController는 한 쌍이 처음부터 화면에 나타나지만 PMenuController는 한 쪽 UI만 처음에 나타남
- 이후 Space 입력시 나머지 한 쪽 UI를 나타내 진입하며 진입하는 UI가 같은 PMenuController인지 단순히 Space/Esc만 입력받는 UnityEventPauseUI인지를 **if문**으로 구분해 진입


### 🔗 2.1. 클래스 간 관계도
<img src="./images/InputUI관계도.png">

### 🔄 2.2. 실행 절차 흐름
#### `UIPlayerController`
1. WASD로 플레이어를 조작
2. Trigger의 종류에 따라 UI기능 활성화

#### `MenuController`
1. UIPlayer가 TitleScene의 설정 UI 활성화
2. MenuController 좌 우 한 쌍이 같이 나타남
3. W/A를 통해 설정값을 바꿀 UI를 선택 후 D키로 진입
4. W/A를 통해 세부 설정사항을 선택
5. D/A를 통해 값을 변경
6. 값이 바뀔 때마다 Singleton Manager들에게 변경값 전달
7. Esc로 뒤로가기

#### `PMenuController`
1. Esc 키로 Game/Tutorial Scene의 일시정지 메뉴 UI 활성화
2. PMenuController 한 쪽만 오른쪽 상단에 나타남
3. W/A를 통해 설정값을 바꿀 UI 또는 다시하기 등 특정 기능UI를 선택 후 Space키로 진입
4. 중앙에 PMenuController 또는 UnityEventPauseUI가 나타남
5. PMenuController라면 MenuController처럼 조작
6. 값이 바뀔 때마다 Singleton Manager들에게 변경값 전달
7. UnityEventPauseUI라면 Space로 기능 활성화
7. Esc로 뒤로가기

## 🚨 3. 주요 이슈 및 디버깅 사례
### 조작계통 꼬임 문제
#### 발생문제
조작할 UI가 아님에도 입력을 받거나 조작할 UI임에도 입력을 받지않는 문제가 간간히 발생

#### 원인 분석
조작할 UI와 아닌 UI의 구분이 불확실

#### 해결 방법
Bool 변수와 If문으로 현재 조작할 UI를 확실히 구별할 수 있게 설계 및 PMenuController를 MenuController에서 구분하여 독립적인 인풋 시스템 클래스로 생성