# Issue 165 추가 카드/유물 리스트 계획

## 목적

#165는 기존 카드/유물 풀 이후에 붙일 추가 콘텐츠를 확정하고, 구현자가 바로 데이터와 코드에 반영할 수 있는 기준을 만드는 작업이다.

이번 계획은 `Docs/ContentDesign_v0.1.md`의 예시 카드/유물을 우선 참고한다. 모든 카드와 유물이 대단한 능력일 필요는 없다. 다만 같은 역할을 반복해서 늘리지 않고, 각 항목이 덱 안에서 다른 포지션을 갖도록 설계한다.

## 현재 구조 요약

- 카드는 `CardDefinition` ScriptableObject로 관리된다.
- 카드 이름은 `KorCardData.csv`의 `Card{id}Name` 키로 표시된다.
- 카드 설명은 `CardAction.CardDescriptionKey`와 `KorStringData.csv`의 텍스트를 조합해 만든다.
- 현재 `CardDefinition`은 태그를 하나만 가진다. `Docs/ContentDesign_v0.1.md`의 `[공격][화염]` 같은 복수 태그 표기는 대표 태그 1개 + 키워드 플래그로 축약한다.
- 유물은 `RelicBase` 파생 클래스를 `GamePlayData.AllLoadoutRelics`에 직접 등록하는 구조다.
- 현재 유물 훅은 `CalculateAmount`, `CalculateRepeat` 중심이라, 카드 사용, 턴 시작, 보상 선택지 변경 같은 규칙 변형 유물은 훅 추가가 필요하다.

## 추가 카드 30종

| ID | 카드명 | 포지션 | 대표 태그 | 키워드 | 희귀도 | 비용 | 효과 | 구현 난이도 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 18 | 재의 검 | 화상 공격 기본형 | Fire | None | Common | 1 | 피해 8 + 화상 2 | 낮음 |
| 19 | 피의 의식 | 자해 고효율 공격 | Attack | Exhaust | Uncommon | 1 | 자신의 HP 3 감소, 피해 14 | 중간 |
| 20 | 처형 | 조건부 마무리 | Attack | Exhaust | Rare | 2 | 적 HP가 30% 이하이면 즉사, 아니면 피해 6 | 중간 |
| 21 | 사슬 베기 | 다단히트 기본형 | MultiHit | None | Uncommon | 2 | 피해 3을 4회 준다 | 낮음 |
| 22 | 재의 약속 | 귀환 드로우 | Util | Return | Uncommon | 1 | 카드 2장 드로우 | 낮음 |
| 23 | 순교 | 자해 방어 폭발 | Defense | Exhaust | Rare | 0 | 자신의 HP 5 감소, 방어도 20 획득 | 중간 |
| 24 | 방패 밀치기 | 방어도 피해 전환 | Defense | None | Common | 1 | 방어도 6 획득 후 현재 방어도만큼 피해 | 낮음 |
| 25 | 숨 고르기 | 0코스트 순환 | Util | None | Common | 0 | 카드 1장 드로우 | 낮음 |
| 26 | 불씨 확산 | 화상 누적 보조 | Fire | None | Common | 1 | 화상 2 부여. 대상이 이미 화상이면 화상 2 추가 | 중간 |
| 27 | 철퇴 강타 | 고비용 단타 | Attack | None | Common | 2 | 피해 16 | 낮음 |
| 28 | 방벽 세우기 | 유지 방어 | Defense | Retain | Common | 1 | 방어도 7 획득 | 낮음 |
| 29 | 과열 | 과부하 화염 폭딜 | Fire | Overload | Uncommon | 1 | 피해 10 + 화상 3 | 낮음 |
| 30 | 왕명 거부 | 선천 전투 준비 | Util | Innate | Uncommon | 0 | 힘 1 획득, 카드 1장 드로우 | 낮음 |
| 31 | 응급 처치 | 소형 회복 | Util | Exhaust | Uncommon | 1 | HP 4 회복 | 중간 |
| 32 | 도발 | 방어+약화 | Defense | None | Uncommon | 1 | 방어도 5 획득, 대상에게 약화 1 부여 | 낮음 |
| 33 | 기름 붓기 | 화상 조건 딜 | Fire | None | Uncommon | 1 | 대상이 화상이면 피해 12, 아니면 화상 4 | 중간 |
| 34 | 검날 세우기 | 순수 힘 버프 | Util | None | Common | 1 | 힘 2 획득 | 낮음 |
| 35 | 쇄도 | 공격 순환 | Attack | Chain | Uncommon | 1 | 피해 7. 사용 후 카드 1장 즉시 드로우 | 낮음 |
| 36 | 최후의 버팀목 | 체력 조건 방어 | Defense | Exhaust | Rare | 1 | HP가 절반 이하면 방어도 18, 아니면 방어도 8 | 중간 |
| 37 | 재점화 | 귀환 화상 | Fire | Return | Rare | 2 | 화상 5 부여 | 낮음 |
| 38 | 무장 해제 | 방어도 제거 공격 | Attack | None | Uncommon | 1 | 피해 5 + 대상 방어도 제거 | 낮음 |
| 39 | 빈틈 막기 | 무방어 보정 | Defense | None | Common | 1 | 현재 방어도가 0이면 방어도 10, 아니면 카드 1장 드로우 | 중간 |
| 40 | 희생의 불꽃 | 0코스트 화상 폭발 | Fire | Exhaust | Rare | 0 | 자신의 HP 4 감소, 화상 8 부여 | 중간 |
| 41 | 재빠른 찌르기 | 0코스트 소형 공격 | Attack | None | Common | 0 | 피해 3 | 낮음 |
| 42 | 취약 찌르기 | 공격+취약 | Attack | None | Uncommon | 1 | 피해 5 + 취약 1 부여 | 낮음 |
| 43 | 방어 태세 | 선천 방어 | Defense | Innate | Uncommon | 1 | 방어도 10 획득 | 낮음 |
| 44 | 연속 호흡 | 다음 턴 드로우 준비 | Util | None | Uncommon | 1 | 카드 1장 드로우, 다음 턴 드로우 +1 | 중간 |
| 45 | 왕관 파쇄 | 고비용 소모 공격 | Attack | Exhaust | Rare | 3 | 피해 24 | 낮음 |
| 46 | 불꽃 장벽 | 방어+화상 | Fire | None | Uncommon | 2 | 방어도 10 획득, 화상 3 부여 | 낮음 |
| 47 | 피의 복수 | 잃은 체력 참조 공격 | Attack | None | Rare | 1 | 기본 피해 6 + 잃은 HP 20%만큼 추가 피해 | 중간 |

### 카드 구현 기준

- `CardAssetGenerator.CreateCard`가 희귀도, 키워드, 임시 아이콘명을 받을 수 있도록 확장한다.
- 신규 카드 전용 이미지가 없으므로 이번 작업에서는 기존 카드 스프라이트를 임시 재사용한다.
- `Card18Name`부터 `Card47Name`까지 `KorCardData.csv`에 추가한다.
- 필요한 신규 액션은 범용적으로 만든다.
  - `LosePlayerHealthCardAction`
  - `HealPlayerCardAction`
  - `ExecuteCardAction`
  - `ConditionalBurnCardAction`
  - `ConditionalArmorOrDrawCardAction`
  - `NextTurnDrawBonusCardAction`
  - `LostHealthBonusDamageCardAction`
- 전용 카드 아트 제작은 후속 `[ASSET]` 이슈로 분리한다.

## 추가 유물 30종

`Docs/ContentDesign_v0.1.md`의 영구/일회성 유물 예시를 중심으로 선정한다. 기존 `Greatsword`, `Dagger`, `ThickShield`와 같은 단순 공격/다단히트/방어도 상시 보정은 새 유물에서 반복하지 않는다.

| 번호 | 유물명 | 포지션 | 효과 | 코스트 | 필요한 훅/구현 |
| --- | --- | --- | --- | --- | --- |
| 1 | 현자의 안경 | 유틸 에너지 보상 | [유틸] 카드 사용 시 1턴에 한 번 에너지 +1 | 3 | 카드 사용 후 훅, 턴별 발동 여부 초기화 |
| 2 | 회귀의 룬 | 귀환 보상 | [귀환] 카드가 손에 돌아올 때마다 방어도 +2 | 2 | 귀환 카드 복귀 훅 |
| 3 | 소각의 맹세 | 소모 보상 | [소모] 카드 사용 시 카드 1장 드로우 | 2 | 카드 사용 후 키워드 검사 훅 |
| 4 | 욕망의 왕관 파편 | 보상 규칙 변형 | 카드 보상 시 4장 중 1장 선택 | 2 | 카드 보상 선택지 수 보정, 보상 UI 슬롯 확장 |
| 5 | 반역자의 맹세 | 전투 시작 리스크 | 매 전투 시작 시 HP -3, 첫 턴 드로우 +2 | 2 | 전투 시작 훅, 첫 턴 드로우 보정 |
| 6 | 녹슨 가시 갑옷 | 피격 반사 | 받은 피해의 30%를 공격자에게 반사 | 2 | 피격 후 공격자 참조 훅 |
| 7 | 얼어붙은 사슬 | 방어 디버프 | [방어] 카드 사용 시 대상에게 약화 1 부여 | 2 | 카드 사용 후 태그 검사 |
| 8 | 고리의 약속 | 드로우 코스트 보상 | 카드 효과로 드로우된 카드의 비용 -1, 이번 턴 한정 | 3 | 드로우 출처 추적, 임시 비용 보정 |
| 9 | 굶주린 검 | 처치 후 강화 | 적 처치 시 무작위 손패 카드 1장을 이번 전투 한정 강화 | 3 | 적 처치 훅, 카드별 임시 배율 |
| 10 | 재의 인도자 | 화상 처치 회복 | 적이 화상 피해로 처치되면 잃은 HP의 30% 회복 | 3 | 피해 출처 추적, 상태이상 처치 훅 |
| 11 | 잊혀진 자의 책 | 첫 카드 강화 | 매 턴 첫 카드 효과 2배, 그 카드는 즉시 소모 | 3 | 턴별 첫 카드 훅, 일회성 배율, 소모 처리 |
| 12 | 왕관의 입맞춤 | 보스 특화 | 보스에게 가하는 피해 +50%, 일반 적에게 가하는 피해 -25% | 2 | 현재 노드 타입 기반 피해 보정 |
| 13 | 처형대의 사슬 | 반복 사용 누적 | 같은 적에게 같은 카드를 사용할 때마다 효과 +25% 누적 | 3 | 카드/대상 사용 기록 |
| 14 | 무덤의 호흡 | 1회 부활 | HP 0 도달 시 50%로 부활, 손패 카드가 이번 전투 동안 소모 획득 | 3 | 사망 차단 훅, 손패 키워드 임시 추가 |
| 15 | 시간의 모래 | 추가 턴 | 매 5턴마다 적 행동 없이 추가 플레이어 턴 1회 | 3 | 턴 매니저 개입 |
| 16 | 작은 화로 | 첫 화상 강화 | 매 전투 첫 화상 부여량 +2 | 1 | 전투별 발동 여부, 화상 부여량 보정 |
| 17 | 재 보관함 | 화상 후 방어 | 화상 카드를 사용하면 방어도 2 획득 | 1 | 카드 사용 후 Fire 태그 검사 |
| 18 | 전투 북 | 첫 공격 강화 | 매 턴 첫 [공격] 카드 피해 +5 | 2 | 턴별 첫 Attack 검사, 피해 보정 |
| 19 | 납 주사위 | 보상 변동 | 카드 보상 중 Common 1장을 무작위로 다시 뽑음 | 2 | 보상 생성 후 후보 교체 |
| 20 | 낡은 지도 | 맵 보상 | 전투 승리 시 골드 +5, 보스 전투에서는 +15 | 1 | 전투 종료 보상 훅 |
| 21 | 피 묻은 붕대 | 자해 완화 | 카드 효과로 받는 자기 피해 -1 | 2 | 자해 액션 보정 |
| 22 | 연마석 | 처치 드로우 | [공격] 카드로 적 처치 시 카드 1장 드로우 | 2 | 카드 사용 결과 처치 훅 |
| 23 | 무거운 부츠 | 유지 보상 | [유지] 카드가 턴 종료 시 손에 남으면 다음 턴 방어도 +3 | 2 | 턴 종료 손패 검사, 다음 턴 보정 |
| 24 | 마른 화약 | 과부하 보상 | [과부하] 카드 효과 +25% | 2 | Overload 키워드 피해/수치 보정 |
| 25 | 차가운 심장 | 무행동 보상 | 이전 턴에 카드를 사용하지 않았다면 다음 턴 힘 2 획득 | 2 | 턴별 카드 사용 수 기록 |
| 26 | 파수꾼의 종 | 첫 피격 완화 | 매 전투 첫 피격 피해 -5 | 1 | 전투별 첫 피격 훅 |
| 27 | 재빠른 손 | 0코스트 보상 | 매 턴 첫 0비용 카드 사용 시 카드 1장 드로우 | 2 | 카드 비용 검사, 턴별 발동 여부 |
| 28 | 왕실 문장 | 엘리트/보스 보상 | 엘리트/보스 전투 승리 보상 골드 +20 | 2 | 노드 타입 기반 보상 보정 |
| 29 | 깨진 성배 | 회복 보정 | 회복 카드 효과 +2, 전투 시작 시 HP -1 | 2 | 회복량 보정, 전투 시작 훅 |
| 30 | 검은 촛대 | 소모 화상 | [소모] 카드 사용 시 대상에게 화상 2 부여 | 2 | 카드 사용 후 Exhaust 키워드 검사 |

### 유물 구현 기준

- `낡은 방패끈` 같은 단순 Defense 방어도 보정 유물은 제외한다. 기존 `ThickShield`와 역할이 겹치기 때문이다.
- `RelicBase`에 전투 흐름 훅을 추가한다.
  - `OnBattleStart(BattleManager battleManager)`
  - `OnPlayerTurnStart(BattleManager battleManager, int turnNumber)`
  - `OnPlayerTurnEnd(BattleManager battleManager, int turnNumber)`
  - `OnCardUsed(CardUseContext context)`
  - `OnReturnedCardToHand(BattleManager battleManager, CardInstance card)`
  - `ModifyRewardCardCount(MapNodeType nodeType, int count)`
  - `ModifyGoldReward(MapNodeType nodeType, int amount)`
  - `OnEnemyKilled(CardUseContext context, CharacterBase enemy)`
  - `OnAfterOwnerDamaged(CharacterBase owner, CharacterBase attacker, int damageTaken)`
- 기존 `CalculateAmount`, `CalculateRepeat`는 유지한다.
- 신규 유물 아이콘은 기존 유물 아이콘을 임시 재사용할 수 있도록 `RelicBase`에 `iconName` 가상 프로퍼티를 추가한다.
- 유물 이름/설명 키는 `KorStringData.csv`에 `{ClassName}Name`, `{ClassName}Desc` 형식으로 추가한다.
- 한 번에 30종을 모두 구현하되, 구현 복잡도가 높은 유물은 훅만 먼저 만들고 효과 클래스는 명확히 분리해 테스트한다.

## 구현 순서

1. 이 계획 문서를 먼저 커밋 가능한 문서로 확정한다.
2. 카드 구현 기반을 추가한다.
3. 카드 30종을 `CardAssetGenerator`에 등록하고 Unity 메뉴로 에셋을 생성한다.
4. 유물 훅을 `RelicBase`, `RelicManager`, `BattleManager`, `DeckManager`, `GamePlayData`, `CharacterBase`에 연결한다.
5. 유물 30종 클래스를 추가하고 `GamePlayData.AllLoadoutRelics`에 등록한다.
6. 문자열 테이블을 갱신한다.
7. Unity Editor에서 카드 생성 메뉴 실행 후 보상 풀과 로드아웃 표시를 확인한다.

## 검증 기준

- Unity Console에 컴파일 오류가 없어야 한다.
- `Tools/Card/Generate Card Assets` 실행 후 `CardDescription`에 카드 ID 18~47 에셋이 생성되어야 한다.
- 카드 보상 풀에서 신규 카드 30종이 등장해야 한다.
- 로드아웃 화면에 신규 유물 30종이 기존 3종과 함께 표시되어야 한다.
- 신규 카드 설명과 유물 설명이 한글로 표시되어야 한다.
- 유물 효과는 각 포지션별로 최소 1회 이상 실제 전투 흐름에서 재현되어야 한다.

## 후속 이슈 후보

- `[ASSET] 추가 카드/유물 전용 아이콘 제작`
- `[BALANCE] 추가 카드/유물 수치 플레이테스트`
- `[REFACTOR] Relic 훅 구조 정리 및 이벤트 기반 전환`
