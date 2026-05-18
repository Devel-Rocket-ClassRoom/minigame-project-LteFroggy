# 카드 시스템 아키텍처

## 전체 구조 한눈에 보기

```
[카드 플레이]
    │
    ▼
BattleManager.PlayCard(CardDefinition card)
    │
    ├─ ctx.currentCardTag = card.tag
    │
    ├─ 유물(Relic)들이 context 수정
    │   └─ if (ctx.currentCardTag == CardTag.Attack) ctx.damageMultiplier *= 1.2f
    │
    └─ 카드 효과들 순서대로 실행
        └─ foreach effect in card.effects → effect.Execute(ctx)
```

---

## 핵심 클래스

### CardDefinition (ScriptableObject)
카드 하나의 **데이터**를 담는 에셋.

| 필드 | 타입 | 설명 |
|------|------|------|
| cardId | string | 고유 식별자 |
| cardName | string | 표시 이름 |
| ~~description~~ | ~~string~~ | ~~삭제~~ — effects에서 동적 생성으로 대체 |
| cost | int | 에너지 코스트 |
| icon | Sprite | 카드 이미지 |
| tag | CardTag | 카드 분류 태그 |
| needsTarget | bool | 타겟 선택 필요 여부 |
| effects | List\<CardEffect\> | 이 카드가 실행할 효과 목록 |

### CardTag (Enum)
카드의 **분류 라벨**. 실제 행동과 무관함.
- `Attack` — 공격 계열
- `Defense` — 방어 계열
- `MultiHit` — 연타 계열
- `Fire` — 화염 계열
- `Util` — 그 외 유틸

> **중요:** Tag는 라벨이고, 카드의 실제 행동은 `effects` 리스트가 결정한다.
> Util 카드들도 각자 다른 Effect 조합을 가지면 된다.

### CardEffect (abstract ScriptableObject)
카드 **행동 하나**를 표현하는 에셋. 구체 클래스로 구현.

```
CardEffect (abstract)
  ├─ DealDamageEffect     → target에게 피해
  ├─ GetArmorEffect       → player에게 방어막
  ├─ DrawCardEffect       → 카드 드로우
  ├─ ExileCardEffect      → 카드 소멸 (추후)
  └─ DoubleCastEffect     → 이중시전 부여 (추후)
```

각 Effect는 `Execute(BattleContext context)`와 `GetPreviewText(BattleContext ctx)`를 구현한다.

```
// abstract 정의
public abstract void Execute(BattleContext ctx);
public abstract string GetPreviewText(BattleContext ctx);

// 구체 클래스 예시
DealDamageEffect.GetPreviewText(ctx) → "적에게 8의 피해를 입힙니다."
GetArmorEffect.GetPreviewText(ctx)   → "5의 방어막을 획득합니다."
DrawCardEffect.GetPreviewText(ctx)   → "카드를 2장 드로우합니다."
```

### BattleContext
카드 실행 시 **현재 전투 상태**를 담는 컨테이너.

| 필드 | 타입 | 설명 |
|------|------|------|
| player | PlayerCharacter | 플레이어 |
| target | EnemyCharacter | 현재 타겟 |
| currentCardTag | CardTag | 지금 실행 중인 카드의 태그 |
| damageMultiplier | float | 피해 배율 (유물이 수정) |

---

## 카드 실행 흐름 (상세)

```
1. BattleManager.PlayCard(card) 호출

2. ctx.currentCardTag = card.tag
   → 유물이 "어떤 태그의 카드냐"를 알 수 있게 됨

3. 유물 modifier 적용 단계
   foreach (relic in activeRelics)
       relic.ModifyContext(ctx)

   예시: AttackBoostRelic
   → currentCardTag == Attack 이면 damageMultiplier *= 1.2f
   → MultiHit이면 조건 불일치 → 아무것도 안 함

4. 효과 실행 단계
   foreach (effect in card.effects)
       effect.Execute(ctx)

   예시: DealDamageEffect
   → int final = Mathf.RoundToInt(amount * ctx.damageMultiplier)
   → ctx.target.GetDamage(final)
```

---

## 카드 설명 텍스트 미리보기

`CardDefinition.description` 필드는 사용하지 않는다.
카드 UI가 텍스트를 표시할 때 `effect.GetPreviewText(ctx)`를 직접 호출해 동적으로 생성한다.

```
// 카드 UI에서
string preview = string.Join("\n", card.effects.Select(e => e.GetPreviewText(ctx)));
descriptionText.text = preview;
```

**타겟 없을 때 (기본 표시):**
- 중립 BattleContext 사용 (modifier 없음)
- 기본 수치 그대로 표시: "적에게 6의 피해를 입힙니다."

**타겟 호버 시 (실제 수치 반영):**
```
1. preview BattleContext 생성 (hoveredEnemy를 target으로)
2. 유물들이 ModifyContext(ctx) 호출
3. effect.GetPreviewText(ctx) 호출 → 수정된 수치 반영
4. 카드 UI 텍스트 갱신: "적에게 8의 피해를 입힙니다."
```

각 Effect가 자기 문장을 책임지므로 카드 UI는 조립만 하면 된다.

---

## 유물(Relic) 연동 방식

유물은 `ModifyContext(BattleContext ctx)` 를 구현한다.
Effect는 유물의 존재를 전혀 몰라도 된다.

```
Relic (abstract)
  └─ ModifyContext(BattleContext ctx)  ← 여기서 ctx 필드를 조건부로 수정

예:
  AttackBoostRelic : ctx.currentCardTag == Attack → damageMultiplier *= 1.2
  FireBoostRelic   : ctx.currentCardTag == Fire   → damageMultiplier *= 1.5
  GlobalBoostRelic : 조건 없이                   → damageMultiplier *= 1.1
```

---

## 기본 카드 10장 목록

| 카드명 | tag | effects | GetPreviewText 결과 |
|--------|-----|---------|---------------------|
| 타격 | Attack | DealDamageEffect(6) | "적에게 6의 피해를 입힙니다." |
| 타격 | Attack | DealDamageEffect(6) | "적에게 6의 피해를 입힙니다." |
| 타격 | Attack | DealDamageEffect(6) | "적에게 6의 피해를 입힙니다." |
| 타격 | Attack | DealDamageEffect(6) | "적에게 6의 피해를 입힙니다." |
| 방어 | Defense | GetArmorEffect(5) | "5의 방어막을 획득합니다." |
| 방어 | Defense | GetArmorEffect(5) | "5의 방어막을 획득합니다." |
| 방어 | Defense | GetArmorEffect(5) | "5의 방어막을 획득합니다." |
| 방어 | Defense | GetArmorEffect(5) | "5의 방어막을 획득합니다." |
| 집중 | Util | DrawCardEffect(2) | "카드를 2장 드로우합니다." |
| 회피 | Defense | GDD 확인 필요 | — |

> **타격 4장**: 수치가 같으면 DealDamageEffect(6) 에셋 하나를 4장이 공유해도 된다.

---

## 에셋 파일 구조 (Unity Project)

```
Assets/
├─ Resources/
│   ├─ Cards/
│   │   ├─ Card_Strike_1.asset
│   │   ├─ Card_Strike_2.asset
│   │   ├─ Card_Strike_3.asset
│   │   ├─ Card_Strike_4.asset
│   │   ├─ Card_Defend_1.asset
│   │   ├─ Card_Defend_2.asset
│   │   ├─ Card_Defend_3.asset
│   │   ├─ Card_Defend_4.asset
│   │   ├─ Card_Focus.asset
│   │   └─ Card_Dodge.asset
│   └─ Effects/
│       ├─ Effect_DealDamage_6.asset
│       ├─ Effect_GetArmor_5.asset
│       └─ Effect_DrawCard_2.asset
└─ Scripts/
    ├─ Card/
    │   ├─ CardDefinition.cs
    │   ├─ CardEffects.cs        (CardEffect abstract)
    │   ├─ DealDamageEffect.cs
    │   ├─ GetArmorEffect.cs
    │   └─ DrawCardEffect.cs     (추가 필요)
    └─ Battle/
        ├─ BattleContext.cs
        ├─ PlayerCharacter.cs
        └─ EnemyCharacter.cs
```
