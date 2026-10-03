# [2026-10-03] 업무 일지

### ☑️ 오늘 한 일 (Done)
- Title Scene UI 배치
- Title Scene StartGame 루트 UI 구현
- Title Scene Setting UI 베이스 구현
- Undo, Quit 구현
---

### 📋 내일 해야 할 일 (To Do)

- Title Scene ProtoType 완성

---

### 🚨 막힌 부분 / 발생한 이슈 
- **이슈:** Undo와 다른 UI가 동시에 작동

- **원인/시도:** 
원인: Interact UI 작동시 SetActive가 비활성화되며 _isOntrigger가 false되지 않아
상호작용시 마지막 활성화된 UI가 작동
/
시도: OnDisable에서 false로 바뀌도록 변경

---

### 💬 PR 요청 및 리뷰 

- **PR:** x

- **Reviewer:** x