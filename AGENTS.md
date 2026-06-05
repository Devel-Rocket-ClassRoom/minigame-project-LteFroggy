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

## GitHub PR 작성 규칙

PR을 생성하거나 PR 본문을 작성할 때는 별도 지시가 없는 한 관련 이슈를 자동으로 닫는 키워드를 반드시 포함한다.

- 관련 이슈 번호가 명확하면 PR 본문에 `Close #이슈번호` 형식으로 작성한다.
- 여러 이슈를 닫아야 하면 각 이슈 번호를 모두 `Close #이슈번호` 형식으로 적는다.
- 사용자가 이슈를 닫지 말라고 명시한 경우에만 closing keyword를 제외한다.
- 이슈 번호가 확정되지 않은 상태라면 PR 본문 작성 또는 PR 생성 전에 번호를 확인한다.

---

## 채팅 이름 변경 규칙

자신이 작업할 이슈가 확정되면, 현재 동작 중인 채팅의 이름을 아래 형식으로 변경한다.

- 형식: `#이슈번호 정확한이슈이름`
- 이슈 이름은 GitHub에 등록된 정확한 제목을 그대로 사용한다.
- 예: `#118 [FEAT] 피격 시 파티클 및 데미지 숫자 표시`

---

## 워크트리 폴더명 규칙

새 워크트리를 만들 때는 반드시 아래 형식으로 폴더명을 만든다.

- 형식: `{프로젝트명}-codex-{issue-type}-{issue-number}-{issue-name}`
- `프로젝트명`은 현재 레포 디렉터리 이름을 사용한다.
- `issue-type`은 이슈 성격을 영어 소문자로 쓴다. 예: `feature`, `bug`, `chore`, `docs`, `refactor`, `asset`
- `issue-number`는 GitHub 이슈 번호만 쓴다.
- `issue-name`은 이슈 제목의 핵심 문구를 파일명에 안전한 kebab-case로 줄여 쓴다.
- 예: `minigame-project-LteFroggy-codex-feature-135-player-damage-feedback`

---

## 커밋 메시지 작성 규칙

- 작성 전 반드시 `git log --oneline`으로 기존 형식을 확인한다.
- 세부항목은 what보다 **why** 중심으로 작성한다.

---

## 커밋 단위 분리

**사용자가 커밋을 요청하면, 모든 변경 파일을 한 번에 커밋하지 말고 작업 단위별로 나눠서 커밋한다.**

- 변경 사항을 논리적 작업 단위로 묶어 여러 개의 커밋으로 분리한다.
- 서로 무관한 변경을 하나의 커밋에 섞지 않는다.

---

## 커밋 타입 의미 기반 선택

**커밋 타입은 기존 히스토리를 맹목적으로 복사하지 말고, 타입의 의미에 맞게 선택한다.**

- `feat`: 새로운 기능 추가
- `fix`: 버그 수정
- `docs`: 문서 변경 (README, CLAUDE.md, AGENTS.md 등)
- `refactor`: 기능 변경 없는 코드 구조 개선
- `asset`: 에셋 추가/수정
- `chore`: 빌드·툴링·설정 유지보수 (.gitignore, CI 등 소스/문서 외 변경)
- 기존 커밋에 잘못된 타입이 쓰였더라도 그대로 따르지 않는다.

---

## 응답 톤 규칙

- 이 레포에서 사용자에게 답변할 때는 항상 존댓말을 사용한다.
- 반말, 낮춤말, 지나치게 캐주얼한 말투를 사용하지 않는다.

---

## GitHub CLI 인증 처리 규칙

Codex 샌드박스 내부 일반 실행에서 `gh auth status`가 invalid token으로 보이더라도, 사용자의 Windows keyring 인증은 정상일 수 있다.

- GitHub 권한이 없다는 메시지가 나오면 먼저 샌드박스 내부 인증 문제로 판단하고, 토큰을 요구하지 말고 `require_escalated` 승인 실행으로 같은 명령을 다시 시도한다.
- `gh auth status`, `gh pr create`, `gh pr merge`, `gh pr checks`, `gh repo view` 등 GitHub CLI 인증이 필요한 명령은 `require_escalated` 승인 실행으로 재확인한다.
- 승인 실행에서 keyring 인증이 정상으로 확인되면, PR 생성/머지 등 GitHub 작업도 같은 방식으로 진행한다.
- 필요하면 같은 종류의 GitHub CLI 작업을 반복 승인 없이 처리할 수 있도록 적절한 `prefix_rule`을 제안한다.
- 사용자가 토큰을 채팅에 직접 붙여넣도록 요구하지 않는다.
