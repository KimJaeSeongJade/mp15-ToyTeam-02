# Input UI

## 📝 1. 기능 개요

### 1.1. 기능 정의

- MenuController와 PMenuController Input System을 통해 키보드 입력만으로 조작하는 UI

### 1.2. 구현 목표

- W/S 키를 조작해 UI선택
- A/D 키를 조작해 값을 변경 및 Singleton Manager들에게 변경된 값 전달

### 1.3. 클래스 설명

- `PanelUI`:
    - 설정할 기능의 이름이 적힌 Text와 현재 선택중인 UI임을 표시할 Image로 구성
    - UIConnector.Cs로 UI를 등록해 연결된 Slider / Slider UI 저장
    - 설정할 기능의 이름을 표시

- `SliderUI`:
    - 1개의 Image, Slider, 세부 항목 이름 Text, 해당 값을 Text로 변환해 나타내는 TextNumChanger컴포넌트를 가진 Text로 구성
    - A/D 키로 Slider를 조작해 값을 변경하는 UI

- `LRUI`:
    - 1개의 Image, LRUI, 세부 항목 이름이 적힌 Text로 구성
    - A/D 키로 좌우 페이지를 넘겨 값을 변경하는 UI

- `UnityEventPauseUI`:
    - 선택된 UI의 기능을 안내하는 문구가 적힌 Text, Image와 작동 Key가 안내된 Image, Text로 구성
    - 자체로 Controller의 역할도 어느정도 수행함
    - Space키로 연결된 UnityEvent기능 활성화

## 🛠️ 2. 작성 클래스 설계 및 구조 

### `PanelUI`
- PMenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- PMenuController로 D키 입력시 UIConnector에 연결된 UI의 MenuController로 작동권을 넘김

### `SliderUI`
- PMenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- PMenuController에 의해 A/D키로 Slider의 값을 변경 및 Singleton Manager들에게 변경된 값 전달

### `LRUI`
- PMenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- PMenuController에 의해 A/D키로 LRUI의 페이지을 변경 Singleton Manager들에게 변경된 값 전달

### `UnityEventPauseUI`
- UI가 담당할 기능을 등록할 UnityEvent 필드 보유
- Space키로 연결된 UnityEvent기능 활성화
- Esc로 저장된 PMenuController로 돌아가기

### 🔗 2.1. 클래스 간 관계도
<img src="./images/InputUI관계도.png">

### 🔄 2.2. 실행 절차 흐름
Input System 문서와 연동

## 🚨 3. 주요 이슈 및 디버깅 사례
### 각종 키 입력 중복 문제 발생
#### 발생문제
MenuController에서 연결된 MenuController을 닫기위해 Esc키 입력시
처음 MenuController가 열려있음에도 Esc입력시 Puase가 풀리거나
UI선택 후 진입시 진입한 UI의 Controller의 입력이 되지 않는 상황 발생

#### 원인 분석
GameSceneManager와 MenuController의 Input 체계가 구분되지 않아 동시에 작동

#### 해결 방법
Bool과 If문을 사용해 조작체계를 확실히 구분 및 PMenuController를 MenuController에서 구분해 독립적인 클래스로 생성하여
Scene마다 필요한 Input 클래스 별개 적용
