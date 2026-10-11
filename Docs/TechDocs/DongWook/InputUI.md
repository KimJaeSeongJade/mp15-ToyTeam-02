# Input UI

## 📝 1. 기능 개요

### 1.1. 기능 정의

- MenuController와 PMenuController를 통해 키보드 입력만으로 조작하는 UI

### 1.2. 구현 목표

- W/S 를 조작해 UI선택
- A/D 를 조작해 값을 변경

### 1.3. 클래스 설명

- `PanelUI`:
    - 설정할 기능의 이름이 적힌 Text와 선택된 UI임을 표시할 Image로 구성
    - UIConnector.Cs로 연결된 Slider / Slider UI 저장
    - 설정할 기능의 이름을 표시

- `SliderUI`:
    - 1개의 Image, Slider, 세부 항목 이름 Text, 해당 값을 숫자로 나타내는 TextNumChanger.cs를 가진 Text로 구성
    - A/D 키로 Slider를 조작해 값을 변경하는 UI

- `LRUI`:
    - 1개의 Image, LRUI, 세부 항목 이름 Text로 구성
    - A/D 키로 페이지를 넘겨 값을 변경하는 UI

## 🛠️ 2. 작성 클래스 설계 및 구조 

### `PanelUI`
- MenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- MenuController로 D키 입력시 UIConnector에 연결된 UI로 작동권을 넘김

### `SliderUI`
- MenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- MenuController에 의해 A/D키로 Slider의 값을 변경 및 Singleton Manager들에게 값 전달

### `LRUI`
- MenuController에 의해 W/S키로 현재 선택 여부에 따라 Image를 끄고 켬
- MenuController에 의해 A/D키로 LRUI의 페이지을 변경 Singleton Manager들에게 값 전달

### 🔗 2.1. 클래스 간 관계도
<img src="./images/InputUI관계도.png">

### 🔄 2.2. 실행 절차 흐름
Input System 문서와 연동

## 🚨 3. 주요 이슈 및 디버깅 사례
없음