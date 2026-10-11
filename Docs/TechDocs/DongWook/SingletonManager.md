# Singleton Manager
## 📝 1. 기능 개요

### 1.1. 기능 정의

- 게임 씬 전체에 존재하며 데이터를 저장하고 돌려주는 Singleton패턴의 Manager 클래스

### 1.2. 구현 목표

- UI의 값 변경에 따른 데이터를 저장 및 반영
- 게임 플레이의 과정 및 결과에서 나오는 데이터를 저장 및 돌려줌
- 각각의 SceneManager들과 연동하여 Scene관리

### 1.3. 클래스 설명

- `GameManager`:
    - Singleton Class로 게임씬 전체에 존재
    - 선택된 게임 모드/난이도, 클리어/오버 판정, 플레이 시간/ 거리점수 등 게임에 관련된 여러 값들을 저장하고 관리하는 Manager

- `SceneManagerA`:
    - Singleton Class로 게임씬 전체에 존재
    - 각각의 Intro, Title, End Scene과 상호작용하며 씬과 관련된 여러 상태들을 관리하고 다루는 Manager

## 🛠️ 2. 작성 클래스 설계 및 구조 

### `GameManager`
- GameManager.Instance 로 게임씬 전체에서 접근가능
- GameManager의 각종 Property에 접근하여 데이터를 저장 및 불러옴

### `SceneManagerA`
- SceneManagerA.Instance 로 게임씬 전체에서 접근가능
- UI에서 변경된 해상도 및 창모드를 적용
- UI에서 변경된 Bgm 및 Sfx Volume을 적용
- UI에서 호출한 LoadScene으로 Scene변경

### 🔗 2.1. 클래스 간 관계도
<img src="./images/SingletonManager관계도.png">

### 🔄 2.2. 실행 절차 흐름
1. .Instance로 다른 코드에서 호출
2. Property에 각종 데이터 저장 및 불러오기
3. 각각의 기능이 담긴 메서드 호출

## 🚨 3. 주요 이슈 및 디버깅 사례
없음