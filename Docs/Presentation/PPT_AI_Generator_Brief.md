# Break The Crown 발표 PPT 생성용 브리프

아래 내용을 PPT 생성 AI에 그대로 넣어 사용할 수 있는 형태로 정리했습니다. 발표 시간은 10~15분, 슬라이드 수는 10장 기준입니다. 발표의 중심은 기술 발표가 아니라 **내가 만든 게임 소개**이며, 기술 설명은 게임 완성도를 뒷받침하는 보조 파트로 배치합니다.

실제로 넣을 이미지 후보는 `Docs/Presentation/PPT_Image_Asset_List.md`에 별도로 정리했습니다. PPT 생성 AI에 전달할 때는 이 브리프와 이미지 목록 문서를 함께 제공하는 것을 추천합니다.

---

## 전체 요청 프롬프트

Unity로 제작한 카드 덱빌딩 로그라이크 게임 **Break The Crown**을 소개하는 10장짜리 발표 PPT를 만들어주세요.

발표의 메인은 게임 소개입니다. 기술 발표처럼 보이지 않게 해주세요. 다만 제가 구현 과정에서 신경 쓴 설계 포인트 3가지는 중간 파트에서 자연스럽게 보여주세요.

강조할 설계 포인트는 다음 3가지입니다.

1. 카드 액션을 작은 단위로 분리하고, 여러 CardAction을 합성해 하나의 카드를 구성한 점
2. 카드 설명 텍스트와 실제 카드 효과가 같은 액션 구조에서 출발하도록 만든 점
3. 설명 패널과 키워드 강조를 통해 플레이어가 복잡한 카드/상태이상 규칙을 이해하기 쉽게 만든 점

유물 훅 기반 확장성도 장점이지만, 메인 강조보다는 “확장을 고려한 보조 설계” 정도로 다뤄주세요.

전체 톤은 게임 발표용으로 시각적이고 흥미롭게 구성하되, 학교/프로젝트 발표에 어울리게 너무 과장된 마케팅 스타일은 피해주세요. 어두운 판타지 덱빌딩 게임 분위기를 살리고, 카드/유물/전투/맵 진행 이미지를 적극적으로 사용해주세요.

디자인 방향:

- 배경: 어두운 왕국, 성, 왕관, 불꽃, 카드 전투 느낌
- 색상: 다크 브라운/블랙/레드/골드 중심, 보조로 차분한 베이지 사용
- 폰트: 제목은 굵고 판타지 느낌, 본문은 읽기 쉬운 고딕 계열
- 각 슬라이드는 한 문장 핵심 메시지가 분명해야 함
- 기술 코드는 최소화하고, 구조 다이어그램과 게임 화면/에셋 위주로 표현

---

## 슬라이드별 구성

### 1. 표지: Break The Crown

**핵심 메시지**  
카드와 유물 조합으로 왕관을 깨는 덱빌딩 로그라이크 게임

**슬라이드 텍스트**

제목:
Break The Crown

부제:
카드 선택과 유물 조합으로 왕관을 깨는 덱빌딩 로그라이크

작은 설명:
플레이어는 전투를 반복하며 카드를 강화하고, 유물을 조합해 자신만의 전략을 만들어갑니다.

**넣을 이미지**

- 배경 이미지: `Assets/Sprites/Backgrounds/BattleArena_ThroneRoom.png`
- 플레이어: `Assets/Sprites/Player/Player.png`
- 보스: `Assets/Sprites/Enemies/Boss/의식의 신수.png`

**디자인 지시**

왕좌의 방 배경을 크게 깔고, 플레이어와 보스를 대치시키는 구도로 배치해주세요. 제목은 왼쪽 상단 또는 중앙에 크게 배치합니다.

---

### 2. 게임 장르와 목표

**핵심 메시지**  
이 게임은 매 턴 손패와 적 의도를 읽고 최적의 선택을 하는 전략 카드 게임입니다.

**슬라이드 텍스트**

제목:
손패와 적 의도를 읽고, 가장 좋은 선택을 하는 게임

본문:
Break The Crown은 덱빌딩 로그라이크 방식의 카드 전투 게임입니다.  
플레이어는 매 턴 제한된 에너지 안에서 카드를 사용하고, 적의 다음 행동을 예측하며 전투를 풀어나갑니다.

강조 문장:
카드 한 장의 강함보다, 카드 조합과 상황 판단이 중요합니다.

**넣을 이미지**

- 카드 이미지 예시:
  - `Assets/Sprites/Cards/AshBlade.png`
  - `Assets/Sprites/Cards/FlameBarrier.png`
  - `Assets/Sprites/Cards/CrownBreaker.png`
  - `Assets/Sprites/Cards/Whirlwind.png`
- 적 의도 아이콘:
  - `Assets/Resources/Sprites/Intents/Attack.png`
  - `Assets/Resources/Sprites/Intents/Defend.png`
  - `Assets/Resources/Sprites/Intents/Burn.png`

**디자인 지시**

왼쪽에는 카드 3~4장을 펼쳐 놓고, 오른쪽에는 적 의도 아이콘을 배치해주세요. “선택”, “예측”, “조합”이라는 키워드가 보이게 구성하면 좋습니다.

---

### 3. 전체 플레이 루프

**핵심 메시지**  
게임은 준비, 맵 선택, 전투, 보상, 보스전으로 이어지는 하나의 루프입니다.

**슬라이드 텍스트**

제목:
준비 - 선택 - 전투 - 보상 - 보스로 이어지는 플레이 루프

단계:
1. 로드아웃에서 시작 카드와 유물 선택
2. 맵에서 다음 노드 선택
3. 전투에서 카드를 사용해 적 처치
4. 보상으로 카드와 자원 획득
5. 보스전과 엔딩으로 마무리

**넣을 이미지**

- 우선 추천 생성 이미지:
  - `Docs/Presentation/images/generated/generated-gameplay-loop-map.png`
- 맵 노드:
  - `Assets/Sprites/Map/Start.png`
  - `Assets/Sprites/Map/Normal.png`
  - `Assets/Sprites/Map/Rest.png`
  - `Assets/Sprites/Map/Boss.png`
- 유물 예시: `Assets/Resources/Sprites/Relics/Greatsword.png`

**디자인 지시**

생성 이미지를 배경 또는 중심 비주얼로 사용하고, 그 위에 `로드아웃 -> 맵 진행 -> 전투 -> 보상 -> 보스` 텍스트를 얹어주세요. 실제 맵 노드 이미지는 각 단계의 보조 아이콘으로 사용합니다. 마지막 보스 단계가 가장 강하게 보이게 해주세요.

---

### 4. 구현한 주요 콘텐츠

**핵심 메시지**  
단순 전투 기능뿐 아니라 카드, 유물, 적, 상태이상, UI 피드백까지 하나의 플레이 경험으로 연결했습니다.

**슬라이드 텍스트**

제목:
제가 만든 것은 전투 화면 하나가 아니라, 플레이 가능한 게임 루프입니다

구현 범위:
- 카드와 카드 효과
- 유물과 보상 구조
- 적 데이터와 행동 패턴
- 상태이상과 전투 계산
- 설명 패널과 키워드 강조
- 피격 파티클과 데미지 숫자 피드백

**넣을 이미지**

- 일반 적:
  - `Assets/Sprites/Enemies/Normal/EnchantedArmor.png`
  - `Assets/Sprites/Enemies/Normal/RoyalAlchemist.png`
  - `Assets/Sprites/Enemies/Normal/ManaGolem.png`
  - `Assets/Sprites/Enemies/Normal/SawbladeAutomaton.png`
- 상태 아이콘:
  - `Assets/Resources/Sprites/Statuses/BurnIcon.png`
  - `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
  - `Assets/Resources/Sprites/Statuses/StrengthIcon.png`

**디자인 지시**

4개 영역 카드로 구성해주세요. “Cards / Relics / Enemies / UX” 같은 큰 카테고리로 나누고, 각 영역에 대표 이미지를 넣어주세요.

---

### 5. 핵심 설계 1: 카드 액션 합성 구조

**핵심 메시지**  
카드를 하나의 거대한 조건문으로 만들지 않고, 작은 액션들을 조합하는 방식으로 구성했습니다.

**슬라이드 텍스트**

제목:
카드는 작은 CardAction들의 조합으로 만들어집니다

본문:
각 카드는 여러 개의 CardAction을 가질 수 있습니다.  
예를 들어 “피해를 준다”, “화상을 부여한다”, “카드를 뽑는다”, “방어도를 얻는다” 같은 기능을 작은 액션 단위로 분리했습니다.

강조:
새 카드를 만들 때 기존 액션을 조합할 수 있어 확장과 유지보수가 쉬워졌습니다.

구조 예시:
CardDefinition  
→ List<CardAction>  
→ DealDamage / Burn / Draw / Armor / Conditional Effect

**넣을 이미지**

- 카드 이미지:
  - `Assets/Sprites/Cards/Ignite.png`
  - `Assets/Sprites/Cards/FlamingMultiHit.png`
  - `Assets/Sprites/Cards/Overheat.png`

**디자인 지시**

카드 1장이 여러 액션 블록으로 분해되는 다이어그램을 만들어주세요. 기술 코드보다는 “카드 = 액션 조합”이라는 시각적 구조가 먼저 보이게 해주세요.

---

### 6. 핵심 설계 1의 장점: 설명과 효과의 일치

**핵심 메시지**  
카드 설명 텍스트와 실제 실행 효과가 같은 액션 구조에서 나오도록 설계했습니다.

**슬라이드 텍스트**

제목:
설명 텍스트와 실제 효과가 같은 구조에서 출발합니다

본문:
CardAction은 실제 효과를 실행하는 Execute 기능뿐 아니라, 카드 설명을 만들어내는 기능도 함께 가집니다.  
그래서 카드 효과를 추가하면 설명 텍스트도 같은 액션 기준으로 관리할 수 있습니다.

강조 문장:
플레이어에게 보이는 설명과 실제 전투 결과가 어긋나지 않게 만드는 것이 중요했습니다.

간단한 예시:
- DealDamageAction: 피해량 설명 + 실제 피해 적용
- BurnAction: 화상 설명 + 화상 상태 부여
- DrawCardAction: 카드 드로우 설명 + 실제 드로우 실행

**넣을 이미지**

- 카드 이미지:
  - `Assets/Sprites/Cards/AshBlade.png`
  - `Assets/Sprites/Cards/BurnAccelerate.png`
  - `Assets/Sprites/Cards/ChainSlash.png`

**디자인 지시**

왼쪽에는 카드 설명 UI처럼 보이는 박스, 오른쪽에는 실제 효과 적용 흐름을 배치해주세요. 가운데에 “same action source” 또는 “같은 액션 구조”를 연결선으로 표현해주세요.

---

### 7. 핵심 설계 2: 설명 패널과 키워드 강조

**핵심 메시지**  
복잡한 카드 게임일수록 플레이어가 규칙을 바로 이해할 수 있어야 합니다.

**슬라이드 텍스트**

제목:
복잡한 카드 규칙을 읽을 수 있게 만드는 UI

본문:
카드 설명 안의 핵심 키워드를 강조하고, 마우스를 올리면 별도의 설명 패널을 보여주도록 만들었습니다.  
카드, 유물, 상태이상, 적 의도 설명이 모두 이 구조를 활용합니다.

강조 문장:
이 기능은 단순한 UI 장식이 아니라, 플레이어의 학습 비용을 줄이기 위한 장치입니다.

예시:
화상: 턴 시작 시 방어도를 무시하고 중첩 수만큼 피해를 줍니다.

**넣을 이미지**

- 상태 아이콘:
  - `Assets/Resources/Sprites/Statuses/BurnIcon.png`
  - `Assets/Resources/Sprites/Statuses/VulnerableIcon.png`
  - `Assets/Resources/Sprites/Statuses/WeaknessIcon.png`
- 의도 아이콘:
  - `Assets/Resources/Sprites/Intents/Debuff.png`
  - `Assets/Resources/Sprites/Intents/Buff.png`

**디자인 지시**

왼쪽에는 카드 설명 텍스트 박스, 오른쪽에는 툴팁 패널을 보여주세요. 카드 설명 안의 “화상”, “취약”, “약화” 같은 단어가 색상으로 강조된 느낌을 표현해주세요.

---

### 8. 보조 설계: 유물 훅 기반 확장성

**핵심 메시지**  
유물은 전투 흐름의 특정 시점에 개입하는 방식으로 확장됩니다.

**슬라이드 텍스트**

제목:
유물은 전투 흐름에 개입하는 훅으로 확장됩니다

본문:
유물은 전투 시작, 카드 사용 전후, 피해 계산, 보상 선택, 사망 직전 같은 여러 시점에 개입할 수 있습니다.  
덕분에 새로운 유물을 추가할 때 전투 전체 코드를 매번 수정하지 않아도 됩니다.

예시 훅:
- OnBattleStart
- OnBeforeCardUse
- OnAfterCardUse
- ModifyIncomingDamage
- TryPreventOwnerDeath

**넣을 이미지**

- 유물 이미지:
  - `Assets/Resources/Sprites/Relics/SageGlasses.png`
  - `Assets/Resources/Sprites/Relics/ForgottenBook.png`
  - `Assets/Resources/Sprites/Relics/SandsOfTime.png`
  - `Assets/Resources/Sprites/Relics/GraveBreath.png`

**디자인 지시**

전투 흐름 타임라인 위에 유물 아이콘이 각 시점에 꽂히는 느낌으로 표현해주세요. 이 슬라이드는 기술 설명이지만, 코드는 거의 쓰지 말고 구조 그림 위주로 구성해주세요.

---

### 9. 가장 어려웠던 점

**핵심 메시지**  
가장 어려웠던 부분은 기능 하나를 만드는 것보다, 여러 시스템의 계산 기준을 일관되게 맞추는 일이었습니다.

**슬라이드 텍스트**

제목:
어려웠던 점: 서로 다른 시스템의 기준을 맞추는 것

본문:
카드 효과, 유물 보정, 상태이상, 적 의도, 설명 텍스트, 피격 피드백은 각각 따로 보이지만 실제 전투에서는 동시에 연결됩니다.  
그래서 “실제로 적용되는 효과”, “플레이어에게 보이는 설명”, “화면 피드백”이 서로 어긋나지 않게 맞추는 것이 가장 어려웠습니다.

3가지 난점:
- 실제 효과 계산: 피해, 방어도, 상태이상, 유물 보정
- 설명/미리보기: 플레이어가 보는 카드 설명과 키워드
- 화면 피드백: 파티클, 데미지 숫자, 상태/의도 아이콘

**넣을 이미지**

- 우선 추천 생성 이미지:
  - `Docs/Presentation/images/generated/generated-system-consistency.png`
- 피격/전투 느낌 이미지:
  - `Assets/Sprites/Player/Player_Hit_Sheet.png`
  - `Assets/Resources/Sprites/Intents/Attack.png`
  - `Assets/Resources/Sprites/Statuses/BurnIcon.png`

**디자인 지시**

생성 이미지를 배경 또는 중심 다이어그램으로 사용하고, 세 개의 축을 삼각형 또는 세 열 구조로 표현해주세요. 가운데에는 “일관성” 또는 “전투 맥락”이라는 키워드를 배치하면 좋습니다.

---

### 10. 정리와 앞으로의 개선

**핵심 메시지**  
이 프로젝트는 카드 게임의 플레이 루프와 확장 가능한 규칙 구조를 함께 만든 작업입니다.

**슬라이드 텍스트**

제목:
Break The Crown으로 만든 것

정리:
1. 게임 루프  
   카드 선택, 맵 진행, 전투, 보상, 보스전으로 이어지는 플레이 흐름을 만들었습니다.

2. 확장 가능한 카드 구조  
   CardAction을 분리하고 조합해 카드 효과와 설명을 함께 확장할 수 있게 했습니다.

3. 이해를 돕는 UI  
   설명 패널과 키워드 강조로 복잡한 카드 규칙을 플레이어가 읽을 수 있게 했습니다.

앞으로의 개선:
- 카드/유물 밸런싱
- UI 아트 정리
- 유물 보정이 반영되는 카드 미리보기 개선
- 전투 연출과 사운드 보강

**넣을 이미지**

- 왕관/보스/유물 중 하나를 상징 이미지로 사용:
  - `Assets/Sprites/Map/Boss.png`
  - `Assets/Resources/Sprites/Relics/CrownKiss.png`
  - `Assets/Resources/Sprites/Relics/CrownShardOfDesire.png`

**디자인 지시**

마지막 장은 너무 복잡하게 만들지 말고, 세 가지 성과를 큼직하게 정리해주세요. 마지막 문장은 “플레이 가능한 덱빌딩 게임 루프를 만들었다”는 느낌으로 마무리해주세요.

---

## 발표 시간 배분

10~15분 기준으로 추천 시간은 다음과 같습니다.

- 1장: 1분
- 2장: 1분
- 3장: 1분 30초
- 4장: 1분 30초
- 5장: 2분
- 6장: 1분 30초
- 7장: 1분 30초
- 8장: 1분
- 9장: 1분 30초
- 10장: 1분

기술 설명이 길어지면 5~7장에서 시간이 늘어날 수 있으므로, 8장의 유물 훅 구조는 짧게 넘어가는 것을 추천합니다.

---

## 한 문장 발표 요약

이 프로젝트에서 제가 만든 핵심은 카드 전투 하나가 아니라, 카드를 조합하고 맵을 진행하며 유물을 통해 전략을 확장하는 덱빌딩 게임 루프입니다. 그 안에서 카드 액션을 분리하고 합성하는 구조, 설명 텍스트와 실제 효과를 연결하는 구조, 그리고 플레이어가 규칙을 이해하게 돕는 설명 패널 시스템을 설계했습니다.
