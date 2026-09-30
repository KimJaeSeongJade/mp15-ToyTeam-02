# 깃허브 이슈 처리 절차

## ⚠️ 필수 규칙

#### 1. 작업시작하기 전 이슈 생성부터

#### 2. 이슈 생성시 담당자 꼭 지정하기

#### 3. 작업 시작하면 수동으로 이슈를 `In Progress`상태로 바꾸기

#### 4. PR 생성시 본문에 `Close #이슈번호` 쓰기

#### 5. 병합이 끝난 뒤 이슈의 상태를 임의로 번경하지 말기

#### 6. 이슈를 다시 활성화하려면, 이슈창에서 `Reopen`버튼을 활용하기

---
## 🌲 1. 업무 처리 흐름

![업무처리 흐름](https://i.postimg.cc/d1VrTBf7/1-isyucheoli-heuleum.png)

---

## 🚩 2. 이슈 생성 단계
1. 상단 `Issues` 메뉴 -> `New Issue` 버튼 클릭해 새로운 이슈 생성 

![이슈생성 화면](https://i.postimg.cc/K8bN8nmX/2-isyusaengseonghwamyeon.png)

2. 이슈 템플릿과 라벨을 활용하여 내용작성

    - 하단의 `이슈 작성 컨벤션` 참조

<br>

3. 이슈 창 우측의 `Assignees`에서 담당자 지정

![이슈 생성창](https://i.postimg.cc/2jBFdH6h/3-isyusaengseong-chang.png)

<br>

4. 하단의 `Create` 버튼을 누르면 이슈가 생성되고 `프로젝트 보드`의 `Todo` 상태로 지정됨

![이슈 생성 후 확인](https://i.postimg.cc/Gmzk3VMg/4-isyusaengseonghu-bodeuhwag-in.png)

### 이슈 작성 컨벤션


#### 1. 템플릿
- 작성하는 이슈와 매칭되는 템플릿을 선택하여 이슈 작성

#### 템플릿 목록
- `Bug Report` : 오류나 문제점이 발생하면 사용

- `Feature` : 새로운 기능을 구현하거나 추가할 때 사용

- `Docs` : 문서 관련 작업 시 사용

- `Chore` : 폴더 정리 등 기타 작업을 처리할 때 사용

#### 2. 제목

- 작업 한개 단위가 기준, 따라서 제목도 한줄로 작성

- `커밋 메시지 컨벤션`과 동일하게 작성

#### 3. 라벨 지정

-  템플릿 선택시 매칭되는 라벨로 자동 지정


### 하위 이슈 만들기

- 작업 단위가 크다면, 하위 이슈 여러개로 쪼개기 가능

- `Create sub-issue` 버튼을 눌러 하위 이슈 생성 가능

![서브 이슈생성](https://i.postimg.cc/vZm4hBzL/7-seobeu-isyu-mandeulgi.png)

- 연결된 하위 이슈를 모두 처리하면, 상위 이슈 처리 가능해짐

---

## 🔧 3. 진행 상태 변경

### 1. 진행 상태 구분
- `Todo`: 아직 작업을 시작하지 않은 상태, 이슈 생성시 처음 상태로 설정됨

- `In Progress`: 작업 중인 상태, PR을 연결하면 자동으로 `In Progress`상태로 전환됨

- `Done`: 이슈 처리 완료, PR완료휴 병합되면 `Done`상태로 전환됨 

### 2. 진행 상태 컨벤션

- 작업을 시작했다면, **수동으로 직접** `In Progress`상태로 변경한다

- 작업이 완료되지 않았다면, `Done`상태로 **직접 변경하지 않는다**


### 3-1. 이슈 페이지에서 변경하기

- 우측에 위치한 `Projects` 영역의 `Status` 드롭다운에서 선택

![이슈페이지에서 확인](https://i.postimg.cc/7YHhP6pb/5-isyupeijieseo-sangtaehwag-in.png)

### 3-2. 프로젝트 보드에서 변경하기
- 프로젝트의 보드화면으로 이동

![프로젝트 보드 접근](https://i.postimg.cc/xjRjZQq5/6-1-peulojegteubodeu-jeobgeun.png)

- 드래그 앤 드롭으로 바꾸고 싶은 상태 컬럼으로 이동

![프로젝트보드에서 확인](https://i.postimg.cc/dVzJCtS3/6-2-peulojegteu-bodeueseo-sangtaebyeongyeong.png)


---


## 💻 4. 작업 진행과 PR

- `개발 컨번션`에 따라, '작업 단위' == '이슈 1개 생성' == '하위 브랜치 1개 생성' 

### 1. 브랜치 생성하기

- 이슈 창 우측의 `Development` 영역에서  `Create a branch` 버튼을 통해 브랜치 생성 가능

![브랜치생성](https://i.postimg.cc/RVhmbnkh/8-beulaenchisaengseong.png)

### 2. 작업을 진행하고 PR 생성하기

- 병합 대상은 `main` 브랜치로 설정

- PR 본문에는 **반드시** `이슈 닫기 키워드` 기재

![자동이슈 닫기](https://i.postimg.cc/prdw7g1J/9-PRjagseong-gwa-jadong-isyudadgi.png)

- `close / closes / closed / fix / fixes / fixed / resolve / resolves / resolved` 모두 적용 가능

- 이슈가 여러개일 경우 각각 키워드를 적용
- 
    ex) `Closes #12, closes #15` 

### 3. 브랜치 병합

- PR 리뷰 후 브랜치를 병합하면, 이슈가 자동으로 닫히고 `Done` 상태로 변환

---

## 💉 5. `close`를 적지 않았다면

### 병합하기 전인 경우
- PR 본문을 수정해 `close #이슈 번호`를 추가

![PR본문 수정](https://i.postimg.cc/QtfPpfx0/10-PR-bonmun-jigjeobsujeong.png)

<br>

- 또는 이슈 창의 우측 `Developmen` > 톱니바퀴 선택 후 PR을 직접 연결

![이슈창과 PR연결](https://i.postimg.cc/vmzSs09d/11-isyuchang-eseo-PR-yeongyeol.png)

### 이미 병합이 진행된 경우

- 해당 이슈에 들어가 직접 닫기

![이슈창 직접닫기](https://i.postimg.cc/vT1P5MKn/12-isyuchang-jigjeob-dadgi.png)

---

## 🔥 6. 이슈 재활성화

- 끝낸 이슈를 다시 활성화 시키려면 하단의 `Reopne issue` 버튼 사용

![이슈 다시열기](https://i.postimg.cc/ZqTVV6Qc/13-isyu-dasiyeolgi.png)


- 프로젝트 보드에서 `Done`상태인 이슈를 옮길 경우, **상태만 바뀔 뿐** 여전히 이슈는 **닫힌 상태** 임에 주의

