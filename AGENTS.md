# AGENTS.md

AI 에이전트가 이 레포에서 작업할 때 따라야 할 프로젝트별 규칙 모음.
공통 행동 지침은 `.claude/CLAUDE.md`를 참조한다.

---

## 커밋 금지 파일

**폰트 바이너리 파일은 커밋하지 않는다.**

- `.ttf`, `.otf` 등 폰트 원본 파일은 Git에 추가하지 않는다.
- 이유: Dynamic TMP 폰트 에셋의 atlas 캐시가 일부 글자만 포함된 상태로 커밋되면, 런타임에 해당 글자가 □로 깨지는 문제가 발생한다.
- 폰트 파일은 팀 내 별도 경로(Google Drive, Notion 등)로 공유한다.

---

## GitHub Issue 생성 규칙

이슈를 생성할 때는 반드시 레포의 Issue Template과 기존 이슈 형식을 참고하여 그대로 따른다.

- `.github/ISSUE_TEMPLATE/` 내 해당 템플릿을 확인한다.
- 기존 이슈 목록을 보고 제목 형식(`[FEAT]`, `[BUG]`, `[CHORE]` 등), 섹션 구조, 라벨을 맞춘다.
- 이슈 생성 후 GitHub Projects "break-the-crown" (`PVT_kwDODykJwc4BYO7A`)에 추가하고 Status를 Backlog로 설정한다.

---

## 커밋 메시지 작성 규칙

- 작성 전 반드시 `git log --oneline`으로 기존 형식을 확인한다.
- 세부항목은 what보다 **why** 중심으로 작성한다.

---

## 응답 톤 규칙

- 이 레포에서 사용자에게 답변할 때는 항상 존댓말을 사용한다.
- 반말, 낮춤말, 지나치게 캐주얼한 말투를 사용하지 않는다.
