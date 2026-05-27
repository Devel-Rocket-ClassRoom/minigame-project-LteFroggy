# AGENTS.md

Guidelines for AI coding agents working in this repository. Follow these rules together with the user's latest request.

## 1. Core Principles

### Think Before Coding

- State assumptions before implementation.
- If the request has multiple valid interpretations, ask or explain the chosen interpretation.
- Surface tradeoffs when a simpler or safer approach exists.
- Stop and ask when required context is unclear.

### Simplicity First

- Implement only what was requested.
- Avoid speculative features, unnecessary configuration, and single-use abstractions.
- Keep code as small as the task reasonably allows.
- Prefer existing project patterns over new architecture.

### Surgical Changes

- Touch only files and lines directly related to the task.
- Do not refactor adjacent code unless the request requires it.
- Match the existing style, naming, formatting, and structure.
- Remove unused code introduced by your own changes.
- Do not remove pre-existing dead code without explicit permission.

### Goal-Driven Execution

- Convert work into verifiable goals.
- For bug fixes, reproduce or identify the failing behavior before changing code when practical.
- For multi-step work, state a short plan and verify each meaningful step.
- Run relevant checks or explain why they could not be run.

## 2. Work Scope

- Do only the work the user explicitly requested.
- If the user asks for a commit message, write the message only. Do not run `git add` or `git commit`.
- If the user asks for PR text, write the PR body only. Do not run `gh pr create`.
- If the user asks for a review, report findings only. Do not fix code unless asked.
- Ask before doing additional execution, file generation, broad refactoring, or destructive operations.

## 3. Commit Message Rules

Before writing a commit message:

- Run or inspect `git log --oneline` to learn the repository's existing commit style.
- Inspect the relevant diff so the message reflects the actual changes.
- Match the existing language, issue-number style, prefix style, and subject/body format.
- Focus the message on why the change was made, not just what changed.
- When drafting a commit message, include detailed changes below the subject as `-` bullet points.
- For broad diffs, split suggested commits by meaningful change unit rather than producing one large catch-all message.
- Do not invent issue numbers, scopes, or context that are not present in the branch or diff.
- If the user only asked for the message, return the commit message as text and do not commit.

## 4. GitHub Issue Rules

When creating or drafting GitHub issues:

- Check `.github/ISSUE_TEMPLATE/` first and follow the matching template.
- Match existing issue title prefixes such as `[FEAT]`, `[BUG]`, or `[CHORE]` when used.
- Do not add or remove template sections arbitrarily.
- If branch naming depends on an issue number, create or confirm the real issue number first.

Project metadata, when issue creation is explicitly requested:

- Project: `break-the-crown` (`PVT_kwDODykJwc4BYO7A`)
- Status: `Backlog` (`3f8128b9`)
- Iteration IDs:
  - Iteration 1: `84f0f4d3` (2026-05-16 ~ 2026-05-22)
  - Iteration 2: `ae0a886e` (2026-05-23 ~ 2026-05-29)
  - Iteration 3: `8ddad5bf` (2026-05-30 ~ 2026-06-05)

## 5. Communication

- Respond to the user in Korean using polite speech.
- Keep implementation updates short and concrete.
- For code comments, commit messages, issue bodies, and PR text, follow the repository's existing style instead of forcing polite Korean.
- Clearly state what was changed and what was verified.

## 6. Project Context

- This is a Unity project using C#.
- Respect the existing structure under `Assets/`, especially scripts and Unity asset references.
- Avoid editing generated or local Unity folders such as `Library/`, `Temp/`, `Logs/`, `obj/`, and `Builds/` unless explicitly requested.
- Preserve `.meta` files and Unity asset references when moving, renaming, or creating assets.
