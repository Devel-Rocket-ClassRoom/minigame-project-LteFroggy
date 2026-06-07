# Break The Crown PPT용 이미지 정리

이 문서는 발표 PPT에 실제로 넣을 만한 이미지 후보를 슬라이드별로 정리한 목록입니다.  
전문 PPT 생성 AI에 넘길 때는 `Docs/Presentation/PPT_AI_Generator_Brief.md`와 함께 이 문서를 같이 넣으면 됩니다.

---

## 0. 생성 이미지 5장

아래 5장은 이번 발표용으로 새로 생성해 둔 보조 이미지입니다.  
게임의 실제 에셋은 아니므로, **표지/섹션 배경/설계 설명용 보조 이미지**로 사용하는 것을 추천합니다.

### 0.1 표지용 배경

경로:

`Docs/Presentation/images/generated/generated-cover-throne-crown.png`

추천 사용처:

- 1장 표지
- 10장 마무리 배경
- 발표 자료 첫 화면 썸네일

용도:

어두운 왕좌의 방, 바닥의 왕관, 카드가 함께 보여서 `Break The Crown`의 분위기를 한 번에 전달하기 좋습니다. 왼쪽에 어두운 여백이 있어 제목을 올리기 좋습니다.

주의:

실제 게임 스크린샷은 아니므로 “게임 대표 분위기 이미지”로만 사용하고, 실제 게임 구현 소개 장에서는 기존 에셋을 함께 보여주는 것이 좋습니다.

생성 프롬프트 요약:

```text
Dark fantasy throne room, cracked golden crown on stone floor, scattered glowing playing cards,
subtle embers, distant ominous throne, 16:9 wide composition, negative space on the left,
deep charcoal, burgundy, antique gold, no text, no logo, no characters.
```

---

### 0.2 카드 액션 합성 구조 설명용 이미지

경로:

`Docs/Presentation/images/generated/generated-card-action-composition.png`

추천 사용처:

- 5장: 카드 액션 합성 구조
- 6장: 설명 텍스트와 실제 효과의 일치

용도:

여러 카드 조각이 하나의 카드로 합쳐지는 시각이라, `CardAction들을 합성해서 카드를 만든다`는 설명과 잘 맞습니다.

주의:

이미지 자체에 실제 코드 구조가 정확히 들어간 것은 아니므로, PPT에서는 이 이미지를 배경 또는 큰 비주얼로 사용하고, 옆에 `CardDefinition -> List<CardAction>` 다이어그램을 따로 얹는 편이 좋습니다.

생성 프롬프트 요약:

```text
Fantasy cards split into glowing effect fragments, then recombine into one complete battle card,
abstract icons for damage, shield, fire, draw, dark tabletop, magic circles,
16:9, no text, no logo, no characters.
```

---

### 0.3 설명 패널/키워드 강조 설명용 이미지

경로:

`Docs/Presentation/images/generated/generated-keyword-tooltip-ui.png`

추천 사용처:

- 7장: 설명 패널과 키워드 강조
- 9장: 어려웠던 점 중 “설명/미리보기” 파트

용도:

카드 설명 패널과 툴팁 패널이 분리되어 보여서, “플레이어가 복잡한 규칙을 읽게 만드는 UI”를 설명하기 좋습니다.

주의:

실제 게임 UI와 완전히 동일한 이미지는 아니므로, 이 이미지는 “설명용 목업”으로 쓰고 실제 상태 아이콘/카드 이미지를 함께 배치하면 좋습니다.

생성 프롬프트 요약:

```text
Fantasy game UI mockup, card description panel with highlighted keyword areas,
separate glowing tooltip panel, status icons, parchment beige panels,
dark fantasy background, 16:9, no readable text, no logo.
```

---

### 0.4 플레이 루프 설명용 이미지

경로:

`Docs/Presentation/images/generated/generated-gameplay-loop-map.png`

추천 사용처:

- 3장: 전체 플레이 루프
- 4장: 구현한 주요 콘텐츠의 흐름 배경

용도:

로드아웃 카드, 맵, 전투, 보상, 보스로 이어지는 진행 흐름이 한 장 안에 보입니다. 실제 맵 노드 이미지를 따로 배치하지 않아도 발표의 흐름을 빠르게 이해시키기 좋습니다.

주의:

실제 게임 맵 UI와 완전히 같은 이미지는 아니므로, “플레이 흐름을 설명하는 발표용 이미지”로 사용하는 것이 좋습니다.

생성 프롬프트 요약:

```text
Dark fantasy game board path from loadout cards to map nodes to battle to reward chest to boss crown,
parchment map, candlelight, fantasy cards and relic tokens, 16:9, no text, no logo, no characters.
```

---

### 0.5 어려웠던 점 / 시스템 일관성 설명용 이미지

경로:

`Docs/Presentation/images/generated/generated-system-consistency.png`

추천 사용처:

- 9장: 가장 어려웠던 점
- 6장: 설명 텍스트와 실제 효과의 일치

용도:

실제 효과, 설명/미리보기, 화면 피드백이 중앙 전투 맥락으로 연결되는 느낌이 있어 “서로 다른 시스템의 기준을 맞추는 것이 어려웠다”는 메시지와 잘 맞습니다.

주의:

텍스트가 없는 추상 다이어그램이므로, PPT 위에서 `실제 효과`, `설명/미리보기`, `화면 피드백`, `전투 맥락` 같은 라벨을 직접 얹어야 합니다.

생성 프롬프트 요약:

```text
Three connected systems around a central glowing battle context core:
card effects, rule descriptions, visual feedback, dark fantasy technical composition,
abstract panels and icons only, 16:9, no text, no logo, no characters.
```

---

## 1. 슬라이드별 추천 이미지

### 1장. 표지: Break The Crown

우선 추천:

- `Docs/Presentation/images/generated/generated-cover-throne-crown.png`

대체 또는 보조:

- `Assets/Sprites/Backgrounds/BattleArena_ThroneRoom.png`
- `Assets/Sprites/Player/Player.png`
- `Assets/Sprites/Enemies/Boss/의식의 신수.png`

추천 배치:

- 생성 이미지를 전체 배경으로 사용합니다.
- 제목은 왼쪽 어두운 여백에 배치합니다.
- 실제 게임 에셋을 보여주고 싶으면 오른쪽 아래에 플레이어와 보스 이미지를 작게 겹쳐 배치합니다.

---

### 2장. 게임 장르와 목표

추천 이미지:

- `Assets/Sprites/Cards/AshBlade.png`
- `Assets/Sprites/Cards/FlameBarrier.png`
- `Assets/Sprites/Cards/CrownBreaker.png`
- `Assets/Sprites/Cards/Whirlwind.png`
- `Assets/Resources/Sprites/Intents/Attack.png`
- `Assets/Resources/Sprites/Intents/Defend.png`
- `Assets/Resources/Sprites/Intents/Burn.png`

추천 배치:

- 왼쪽에는 카드 3~4장을 부채꼴 또는 손패처럼 배치합니다.
- 오른쪽에는 적 의도 아이콘 3개를 가로로 놓습니다.
- 가운데 문구는 “손패와 적 의도를 읽고 선택하는 게임”으로 잡으면 좋습니다.

---

### 3장. 전체 플레이 루프

추천 이미지:

- `Assets/Sprites/Map/Start.png`
- `Assets/Sprites/Map/Normal.png`
- `Assets/Sprites/Map/Rest.png`
- `Assets/Sprites/Map/Treasure.png`
- `Assets/Sprites/Map/Boss.png`
- `Assets/Resources/Sprites/Relics/Greatsword.png`

추천 배치:

- 가로 타임라인으로 `로드아웃 -> 맵 진행 -> 전투 -> 보상 -> 보스`를 보여줍니다.
- 각 단계마다 위 이미지를 아이콘처럼 사용합니다.
- 마지막 Boss 아이콘은 크기와 색 대비를 조금 더 크게 잡아도 좋습니다.

---

### 4장. 구현한 주요 콘텐츠

추천 이미지:

적 캐릭터:

- `Assets/Sprites/Enemies/Normal/EnchantedArmor.png`
- `Assets/Sprites/Enemies/Normal/RoyalAlchemist.png`
- `Assets/Sprites/Enemies/Normal/ManaGolem.png`
- `Assets/Sprites/Enemies/Normal/SawbladeAutomaton.png`

상태 아이콘:

- `Assets/Resources/Sprites/Statuses/BurnIcon.png`
- `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
- `Assets/Resources/Sprites/Statuses/StrengthIcon.png`

유물:

- `Assets/Resources/Sprites/Relics/CrownShardOfDesire.png`
- `Assets/Resources/Sprites/Relics/SageGlasses.png`
- `Assets/Resources/Sprites/Relics/ForgottenBook.png`

추천 배치:

- `Cards / Relics / Enemies / UX` 네 영역으로 나눕니다.
- 각 영역에 대표 이미지를 1~3개씩 넣습니다.
- 이 장에서는 “많이 만들었다”보다 “플레이 경험을 구성하는 요소들을 연결했다”는 느낌을 주는 것이 좋습니다.

---

### 5장. 카드 액션 합성 구조

우선 추천:

- `Docs/Presentation/images/generated/generated-gameplay-loop-map.png`
- `Docs/Presentation/images/generated/generated-card-action-composition.png`

보조 이미지:

- `Assets/Sprites/Cards/Ignite.png`
- `Assets/Sprites/Cards/FlamingMultiHit.png`
- `Assets/Sprites/Cards/Overheat.png`
- `Assets/Sprites/Cards/ChainSlash.png`

추천 배치:

- 생성 이미지를 오른쪽 또는 배경으로 사용합니다.
- 왼쪽에는 간단한 구조를 넣습니다.

```text
CardDefinition
  -> List<CardAction>
      -> DealDamage
      -> Burn
      -> Draw
      -> Armor
```

핵심 문구:

`카드는 하나의 거대한 조건문이 아니라, 작은 액션들의 조합입니다.`

---

### 6장. 설명 텍스트와 실제 효과의 일치

추천 이미지:

- `Assets/Sprites/Cards/AshBlade.png`
- `Assets/Sprites/Cards/BurnAccelerate.png`
- `Assets/Sprites/Cards/FlameBarrier.png`
- `Assets/Sprites/Cards/FirstAid.png`

보조로 사용 가능:

- `Docs/Presentation/images/generated/generated-card-action-composition.png`

추천 배치:

- 왼쪽에는 카드 설명 UI처럼 보이는 박스를 만듭니다.
- 오른쪽에는 “Execute / GetCardDescription” 두 갈래가 같은 `CardAction`에서 나오는 구조를 다이어그램으로 보여줍니다.

핵심 문구:

`보이는 설명과 실제 효과가 같은 액션 구조에서 출발하도록 만들었습니다.`

---

### 7장. 설명 패널과 키워드 강조

우선 추천:

- `Docs/Presentation/images/generated/generated-keyword-tooltip-ui.png`

보조 이미지:

- `Assets/Resources/Sprites/Statuses/BurnIcon.png`
- `Assets/Resources/Sprites/Statuses/VulnerableIcon.png`
- `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
- `Assets/Resources/Sprites/Intents/Debuff.png`
- `Assets/Resources/Sprites/Intents/Buff.png`

추천 배치:

- 생성 이미지를 크게 사용합니다.
- 실제 상태 아이콘을 오른쪽 또는 아래쪽에 별도로 배치합니다.
- “화상”, “취약”, “약화” 같은 키워드가 설명 패널로 이어지는 흐름을 보여줍니다.

핵심 문구:

`설명 패널은 장식이 아니라, 플레이어의 학습 비용을 줄이는 장치입니다.`

---

### 8장. 유물 훅 기반 확장성

추천 이미지:

- `Assets/Resources/Sprites/Relics/SageGlasses.png`
- `Assets/Resources/Sprites/Relics/ForgottenBook.png`
- `Assets/Resources/Sprites/Relics/SandsOfTime.png`
- `Assets/Resources/Sprites/Relics/GraveBreath.png`
- `Assets/Resources/Sprites/Relics/RustedThornArmor.png`
- `Assets/Resources/Sprites/Relics/CrownKiss.png`

추천 배치:

- 전투 흐름 타임라인 위에 유물 아이콘을 배치합니다.
- 각 위치에는 아래 훅 이름을 짧게 넣습니다.

```text
OnBattleStart
OnBeforeCardUse
OnAfterCardUse
ModifyIncomingDamage
TryPreventOwnerDeath
```

핵심 문구:

`유물은 전투 흐름의 특정 시점에 개입하는 방식으로 확장됩니다.`

---

### 9장. 어려웠던 점

추천 이미지:

- `Assets/Sprites/Player/Player_Hit_Sheet.png`
- `Assets/Resources/Sprites/Intents/Attack.png`
- `Assets/Resources/Sprites/Statuses/BurnIcon.png`
- `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
- `Assets/Sprites/UI/HealthIcon.png`

추천 배치:

- 세 열 구조로 배치합니다.

```text
실제 효과 계산
설명 / 미리보기
화면 피드백
```

- 가운데에 `전투 맥락` 또는 `일관성`을 핵심 키워드로 둡니다.

핵심 문구:

`가장 어려웠던 점은 기능 하나를 만드는 것보다, 여러 시스템의 계산 기준을 맞추는 일이었습니다.`

---

### 10장. 정리와 앞으로의 개선

우선 추천:

- `Docs/Presentation/images/generated/generated-system-consistency.png`
- `Docs/Presentation/images/generated/generated-cover-throne-crown.png`

보조 이미지:

- `Assets/Sprites/Map/Boss.png`
- `Assets/Resources/Sprites/Relics/CrownKiss.png`
- `Assets/Resources/Sprites/Relics/CrownShardOfDesire.png`

추천 배치:

- 생성 표지 이미지를 어둡게 깔고, 위에 세 가지 성과를 정리합니다.

```text
게임 루프
카드 액션 합성 구조
설명 패널 / 키워드 강조
```

핵심 문구:

`카드 게임의 플레이 루프와 확장 가능한 규칙 구조를 함께 만들었습니다.`

---

## 2. 기존 에셋 중 특히 추천하는 이미지 묶음

### 카드 이미지 베스트

- `Assets/Sprites/Cards/AshBlade.png`
- `Assets/Sprites/Cards/CrownBreaker.png`
- `Assets/Sprites/Cards/FlameBarrier.png`
- `Assets/Sprites/Cards/FlamingMultiHit.png`
- `Assets/Sprites/Cards/Ignite.png`
- `Assets/Sprites/Cards/Overheat.png`
- `Assets/Sprites/Cards/Whirlwind.png`
- `Assets/Sprites/Cards/Execute.png`

사용 이유:

시각적으로 공격, 화상, 방어, 강한 카드 느낌이 잘 드러납니다.

---

### 적 캐릭터 이미지 베스트

- `Assets/Sprites/Enemies/Boss/의식의 신수.png`
- `Assets/Sprites/Enemies/Normal/EnchantedArmor.png`
- `Assets/Sprites/Enemies/Normal/ManaGolem.png`
- `Assets/Sprites/Enemies/Normal/RoyalAlchemist.png`
- `Assets/Sprites/Enemies/Normal/SawbladeAutomaton.png`
- `Assets/Sprites/Enemies/Normal/CinderAcolyte.png`

사용 이유:

크기가 1024x1024라 PPT에서 크게 써도 깨지기 어렵고, 게임의 적 다양성을 보여주기 좋습니다.

---

### 배경 이미지 베스트

- `Assets/Sprites/Backgrounds/BattleArena_ThroneRoom.png`
- `Assets/Sprites/Backgrounds/RestRoom.png`
- `Assets/Sprites/Backgrounds/Events/Event_SuspiciousAltar.png`
- `Assets/Sprites/Backgrounds/Events/Event_TreasureChest.png`

사용 이유:

게임의 장소감을 보여주기 좋습니다. 특히 이벤트 배경 2개는 1672x941이라 16:9 슬라이드에 잘 맞습니다.

---

### 유물 이미지 베스트

- `Assets/Resources/Sprites/Relics/Greatsword.png`
- `Assets/Resources/Sprites/Relics/SageGlasses.png`
- `Assets/Resources/Sprites/Relics/ForgottenBook.png`
- `Assets/Resources/Sprites/Relics/SandsOfTime.png`
- `Assets/Resources/Sprites/Relics/GraveBreath.png`
- `Assets/Resources/Sprites/Relics/CrownKiss.png`
- `Assets/Resources/Sprites/Relics/CrownShardOfDesire.png`
- `Assets/Resources/Sprites/Relics/RustedThornArmor.png`

사용 이유:

유물 시스템 설명에서 “전투 흐름에 개입하는 아이템”이라는 느낌을 주기 좋습니다.

---

### UI/아이콘 이미지 베스트

- `Assets/Resources/Sprites/Intents/Attack.png`
- `Assets/Resources/Sprites/Intents/Defend.png`
- `Assets/Resources/Sprites/Intents/Burn.png`
- `Assets/Resources/Sprites/Intents/Buff.png`
- `Assets/Resources/Sprites/Intents/Debuff.png`
- `Assets/Resources/Sprites/Statuses/BurnIcon.png`
- `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
- `Assets/Resources/Sprites/Statuses/VulnerableIcon.png`
- `Assets/Resources/Sprites/Statuses/StrengthIcon.png`

사용 이유:

게임 규칙을 시각적으로 설명하기 좋습니다. 특히 7장 설명 패널/키워드 강조 파트에 잘 맞습니다.

---

## 3. PPT 생성 AI에 추가로 넣을 이미지 관련 지시문

아래 문장을 PPT 생성 AI 프롬프트 끝에 붙이면 좋습니다.

```text
이미지는 가능한 한 실제 프로젝트 에셋을 우선 사용해주세요.
표지와 설계 설명 슬라이드에는 `Docs/Presentation/images/generated` 폴더의 생성 이미지를 보조 배경으로 사용해도 됩니다.
생성 이미지는 실제 게임 화면이 아니라 발표용 분위기/설명 이미지이므로, 게임 구현을 설명하는 장에서는 반드시 실제 카드, 적, 유물, 상태 아이콘 에셋을 함께 보여주세요.
이미지 위에 텍스트를 올릴 때는 어두운 반투명 오버레이를 깔아 가독성을 확보해주세요.
카드/유물/상태 아이콘은 너무 작게 흩뿌리지 말고, 각 슬라이드의 핵심 메시지를 설명하는 대표 이미지로만 사용해주세요.
```

---

## 4. 이미지 사용 우선순위

1. 실제 게임 에셋  
   카드, 적, 유물, 상태 아이콘, 맵 노드, 배경

2. 생성 이미지  
   표지, 섹션 전환, 설계 설명용 보조 배경

3. 직접 만든 도형/다이어그램  
   CardAction 구조, 유물 훅 타임라인, 어려웠던 점의 세 축 설명

이 순서로 쓰면 발표가 “게임 소개” 중심을 유지하면서도 설계 포인트가 자연스럽게 들어갑니다.
