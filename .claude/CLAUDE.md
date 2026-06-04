# CLAUDE.md

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

## 5. GitHub Issue 생성 규칙

이슈를 생성할 때는 반드시 레포의 Issue Template과 기존 이슈 형식을 참고하여 그대로 따른다.

- `.github/ISSUE_TEMPLATE/` 내 해당 템플릿을 확인한다.
- 기존 이슈 목록을 보고 제목 형식(`[FEAT]`, `[BUG]`, `[CHORE]` 등), 섹션 구조, 라벨을 맞춘다.
- 템플릿에 없는 섹션을 임의로 추가하거나 구조를 바꾸지 않는다.
- **작업 브랜치명의 이슈 번호**: 이슈를 먼저 생성해 실제 번호를 받은 뒤, 브랜치명을 해당 번호로 즉시 수정한다. `{이슈번호}` 플레이스홀더를 그대로 남기지 않는다.
- **채팅 이름 변경**: 자신이 작업할 이슈가 확정되면, 현재 동작 중인 채팅의 이름을 `#이슈번호 정확한이슈이름` 형식으로 변경한다. 이슈 이름은 GitHub에 등록된 정확한 제목을 그대로 사용한다. 예: `#118 [FEAT] 피격 시 파티클 및 데미지 숫자 표시`

### Project / Iteration / Milestone

- **Project**: 이슈 생성 후 GitHub Projects "break-the-crown" (`PVT_kwDODykJwc4BYO7A`)에 추가한다.
  - 이슈를 프로젝트에 추가한 뒤, Status를 **Backlog** (`3f8128b9`)로 설정한다.
  - 아래 GraphQL 순서로 처리한다:
    1. `addProjectV2ItemById` 뮤테이션으로 이슈를 프로젝트에 추가 → `itemId` 획득
    2. `updateProjectV2ItemFieldValue` 뮤테이션으로 Status = Backlog 설정
    3. `updateProjectV2ItemFieldValue` 뮤테이션으로 Iteration 설정
- **Iteration**: 요청 날짜(또는 이슈의 마일스톤 기간)에 맞는 Iteration ID를 사용한다.

| Iteration | ID | 기간 |
|---|---|---|
| Iteration 1 | `84f0f4d3` | 2026-05-16 ~ 2026-05-22 |
| Iteration 2 | `ae0a886e` | 2026-05-23 ~ 2026-05-29 |
| Iteration 3 | `8ddad5bf` | 2026-05-30 ~ 2026-06-05 |

- **Milestone**: 요청 날짜에 맞는 마일스톤을 `milestone` 파라미터로 설정한다. 아래 기준 사용:

## 6. 커밋 메시지 작성 규칙

커밋 메시지를 작성하기 전에 반드시 `git log --oneline`으로 기존 커밋 히스토리를 확인하고, 그 형식을 그대로 따른다.

- 형식을 먼저 읽지 않고 임의로 작성하지 않는다.
- 타입, 이슈번호 표기 방식, 언어(한/영), 구두점, 세부항목 나열 방식을 기존과 일치시킨다.
- 세부항목은 **무엇을 했는지(what)보다 왜 그렇게 했는지(why)** 중심으로 작성한다. 변경 내용 자체는 diff로 확인 가능하므로, 그 결정의 이유와 의도를 기록하는 데 집중한다.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.

## 7. 작업 범위 엄수

**사용자가 요청한 작업만 수행한다. 임의로 그 이상을 진행하지 않는다.**

- "커밋 메시지를 써달라"고 하면 메시지 텍스트만 작성해서 보여준다. `git add`, `git commit`을 자동으로 실행하지 않는다.
- "PR 본문을 써달라"고 하면 본문만 작성한다. `gh pr create`를 자동으로 실행하지 않는다.
- "코드를 검토해달라"고 하면 검토 결과만 알려준다. 발견한 문제를 임의로 수정하지 않는다.
- 사용자가 명시적으로 실행을 요청한 경우에만 실제 실행/수정/생성 작업을 수행한다.
- 작업을 더 진행해도 될지 애매하면 멈추고 확인을 받는다.

---

## 8. 한국어 응답 시 존댓말 사용

사용자에게 한국어로 응답할 때는 반드시 존댓말을 사용한다.

- "~합니다", "~하겠습니다", "~할까요?" 형태로 작성한다.
- "~한다", "~할게", "~해" 같은 반말체는 사용하지 않는다.
- 이 규칙은 코드 주석, 커밋 메시지, 이슈/PR 본문에는 적용되지 않는다 (해당 항목은 기존 프로젝트 톤을 따른다).

---

## 9. 커밋 금지 파일

**폰트 바이너리 파일은 커밋하지 않는다.**

- `.ttf`, `.otf` 등 폰트 원본 파일은 Git에 추가하지 않는다.
- 이유: Dynamic TMP 폰트 에셋의 atlas 캐시가 일부 글자만 포함된 상태로 커밋되면, 런타임에 해당 글자가 □로 깨지는 문제가 발생한다.
- 폰트 파일은 팀 내 별도 경로(Google Drive, Notion 등)로 공유한다.

---

## 10. 커밋 단위 분리

**사용자가 커밋을 요청하면, 모든 변경 파일을 한 번에 커밋하지 말고 작업 단위별로 나눠서 커밋한다.**

- 변경 사항을 논리적 작업 단위로 묶어 여러 개의 커밋으로 분리한다.
- 서로 무관한 변경을 하나의 커밋에 섞지 않는다.

---

## 11. 커밋 타입 의미 기반 선택

**커밋 타입은 기존 히스토리를 맹목적으로 복사하지 말고, 타입의 의미에 맞게 선택한다.**

- `feat`: 새로운 기능 추가
- `fix`: 버그 수정
- `docs`: 문서 변경 (README, CLAUDE.md, AGENTS.md 등)
- `refactor`: 기능 변경 없는 코드 구조 개선
- `asset`: 에셋 추가/수정
- `chore`: 빌드·툴링·설정 유지보수 (.gitignore, CI 등 소스/문서 외 변경)
- 기존 커밋에 잘못된 타입이 쓰였더라도 그대로 따르지 않는다.
