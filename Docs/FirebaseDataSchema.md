# Firebase Data Schema

이 문서는 클라이언트가 Firebase Realtime Database에 저장하는 런/메타 진행 데이터 구조를 정리합니다.

## 경로

```text
users/{uid}/metaProgress
users/{uid}/currentRun
users/{uid}/runResults/{pushId}
```

`{uid}`는 Firebase Auth 사용자 ID입니다. 익명 로그인 사용자는 앱 데이터와 Firebase Auth 세션이 유지되는 동안 같은 UID를 사용하며, 이메일 연결을 하면 같은 UID의 진행 데이터를 계속 사용합니다.

## metaProgress

메타 진행 데이터입니다. 메인 메뉴 진입 후 로그인 또는 익명 로그인이 끝나면 `LoadOrCreateMetaProgress`로 로드합니다. 데이터가 없으면 기본 해금 유물 3개로 생성합니다.

| 필드 | 타입 | 설명 |
| --- | --- | --- |
| `gold` | number | 상점에서 사용하는 계정 단위 골드 |
| `unlockedRelicIds` | string[] | 로드아웃에 표시되는 해금 유물 ID |
| `purchasedRelicIds` | string[] | 상점에서 직접 구매한 유물 ID |
| `updatedAtUnixTime` | number | 마지막 저장 시각, UTC Unix seconds |

기본 해금 유물은 `GameContentCatalog.AllLoadoutRelics` 앞 3개입니다.

## currentRun

현재 런 스냅샷입니다. `GameEvents.OnNodeCompleted`가 발생할 때 저장합니다. 저장 실패는 런 진행을 막지 않고 warning 로그만 남깁니다.

| 필드 | 타입 | 설명 |
| --- | --- | --- |
| `currentHealth` | number | 플레이어 현재 체력 |
| `maxHealth` | number | 플레이어 최대 체력 |
| `gold` | number | 현재 런에서 보유 중인 골드 |
| `totalGoldEarned` | number | 현재 런에서 획득한 총 골드 |
| `totalGoldSpent` | number | 현재 런에서 사용한 총 골드 |
| `deckCount` | number | 현재 덱 카드 수 |
| `relicCount` | number | 현재 보유 유물 수 |
| `cardIds` | string[] | 현재 덱 카드 ID |
| `relicIds` | string[] | 현재 보유 유물 ID |
| `currentNodeType` | string | 현재 노드 타입 |
| `currentNodeLayer` | number | 현재 노드 레이어. 찾지 못하면 `-1` |
| `currentNodeIndex` | number | 현재 노드 인덱스. 찾지 못하면 `-1` |
| `visitedNodeTypes` | string[] | 방문 완료 노드 타입 목록 |
| `savedAtUnixTime` | number | 저장 시각, UTC Unix seconds |

## runResults

완료된 런 결과 목록입니다. 패배 또는 보스 승리 시 `Push()`로 새 항목을 추가합니다.

| 필드 | 타입 | 설명 |
| --- | --- | --- |
| `result` | string | `Victory` 또는 `Defeat` |
| `enemyEncounterId` | string | 마지막 전투 인카운터 ID |
| `enemyIds` | number[] | 마지막 전투 적 ID |
| `currentHealth` | number | 런 종료 시 체력 |
| `maxHealth` | number | 최대 체력 |
| `gold` | number | 런 종료 시 보유 골드 |
| `totalGoldEarned` | number | 런 전체 획득 골드 |
| `totalGoldSpent` | number | 런 전체 사용 골드 |
| `deckCount` | number | 종료 시 덱 카드 수 |
| `relicCount` | number | 종료 시 보유 유물 수 |
| `cardIds` | string[] | 종료 시 덱 카드 ID |
| `relicIds` | string[] | 종료 시 보유 유물 ID |
| `savedAtUnixTime` | number | 저장 시각, UTC Unix seconds |

## 골드 반영

런 중 골드는 `GamePlayData.Gold`에 쌓입니다. 전투 보상 골드는 기존 흐름을 유지하고, 보스 클리어 시 100 골드를 추가로 지급합니다. 패배 또는 보스 승리로 런이 끝날 때 현재 런 골드를 `metaProgress.gold`에 더합니다.

## 권장 보안 규칙

Realtime Database 규칙은 사용자별 UID 격리를 전제로 합니다.

```json
{
  "rules": {
    "users": {
      "$uid": {
        ".read": "auth != null && auth.uid == $uid",
        ".write": "auth != null && auth.uid == $uid"
      }
    }
  }
}
```

클라이언트 저장 구조는 위 규칙을 기준으로 동작합니다. Firebase 미설정, 로그인 없음, 네트워크 실패 시에는 게임 진행을 중단하지 않고 warning 로그만 남깁니다.
