import {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  HeadingLevel, AlignmentType, WidthType, BorderStyle, ShadingType,
  LevelFormat, TableOfContents
} from "docx";
import { writeFileSync } from "fs";

const border = { style: BorderStyle.SINGLE, size: 1, color: "AAAAAA" };
const borders = { top: border, bottom: border, left: border, right: border };

function h1(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_1,
    children: [new TextRun({ text, bold: true, size: 32, font: "맑은 고딕" })],
    spacing: { before: 400, after: 200 },
  });
}

function h2(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_2,
    children: [new TextRun({ text, bold: true, size: 28, font: "맑은 고딕" })],
    spacing: { before: 300, after: 160 },
  });
}

function h3(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_3,
    children: [new TextRun({ text, bold: true, size: 24, font: "맑은 고딕" })],
    spacing: { before: 200, after: 120 },
  });
}

function p(text) {
  return new Paragraph({
    children: [new TextRun({ text, size: 22, font: "맑은 고딕" })],
    spacing: { after: 120 },
  });
}

function bullet(text) {
  return new Paragraph({
    numbering: { reference: "bullets", level: 0 },
    children: [new TextRun({ text, size: 22, font: "맑은 고딕" })],
    spacing: { after: 80 },
  });
}

function makeTable(headers, rows, colWidths) {
  const headerRow = new TableRow({
    children: headers.map((h, i) =>
      new TableCell({
        borders,
        width: { size: colWidths[i], type: WidthType.DXA },
        shading: { fill: "2C2C3A", type: ShadingType.CLEAR },
        margins: { top: 80, bottom: 80, left: 120, right: 120 },
        children: [new Paragraph({
          children: [new TextRun({ text: h, bold: true, size: 20, color: "FFFFFF", font: "맑은 고딕" })],
        })],
      })
    ),
  });

  const dataRows = rows.map((row, ri) =>
    new TableRow({
      children: row.map((cell, i) =>
        new TableCell({
          borders,
          width: { size: colWidths[i], type: WidthType.DXA },
          shading: { fill: ri % 2 === 0 ? "F5F5F8" : "FFFFFF", type: ShadingType.CLEAR },
          margins: { top: 80, bottom: 80, left: 120, right: 120 },
          children: [new Paragraph({
            children: [new TextRun({ text: cell, size: 20, font: "맑은 고딕" })],
          })],
        })
      ),
    })
  );

  const total = colWidths.reduce((a, b) => a + b, 0);
  return new Table({
    width: { size: total, type: WidthType.DXA },
    columnWidths: colWidths,
    rows: [headerRow, ...dataRows],
  });
}

function spacer() {
  return new Paragraph({ children: [new TextRun("")], spacing: { after: 120 } });
}

const doc = new Document({
  numbering: {
    config: [{
      reference: "bullets",
      levels: [{
        level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT,
        style: { paragraph: { indent: { left: 720, hanging: 360 } } },
      }],
    }],
  },
  styles: {
    default: { document: { run: { font: "맑은 고딕", size: 22 } } },
  },
  sections: [{
    properties: {
      page: {
        size: { width: 12240, height: 15840 },
        margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 },
      },
    },
    children: [
      // 표지
      new Paragraph({
        children: [new TextRun({ text: "Game Design Document", size: 52, bold: true, font: "맑은 고딕", color: "1A1A2E" })],
        alignment: AlignmentType.CENTER,
        spacing: { before: 2000, after: 200 },
      }),
      new Paragraph({
        children: [new TextRun({ text: "Break the Crown", size: 64, bold: true, font: "맑은 고딕", color: "8B0000" })],
        alignment: AlignmentType.CENTER,
        spacing: { after: 200 },
      }),
      new Paragraph({
        children: [new TextRun({ text: "v0.3", size: 28, font: "맑은 고딕", color: "666666" })],
        alignment: AlignmentType.CENTER,
        spacing: { after: 200 },
      }),
      new Paragraph({
        children: [new TextRun({ text: "턴제 로그라이크 덱빌딩 게임", size: 28, font: "맑은 고딕", color: "444444" })],
        alignment: AlignmentType.CENTER,
        spacing: { after: 200 },
      }),
      new Paragraph({
        children: [new TextRun({ text: "개발 기간: 3주  |  엔진: Unity  |  플랫폼: PC  |  개발: 1인", size: 22, font: "맑은 고딕", color: "888888" })],
        alignment: AlignmentType.CENTER,
        spacing: { after: 2000 },
      }),

      // 1. 프로젝트 개요
      h1("1. 프로젝트 개요"),

      h2("1.1 배경"),
      p("왕국의 모든 권력은 왕관에서 나온다. 왕관은 원래 왕을 선택하는 성물이었지만, 어느 순간부터 왕을 삼키는 저주가 되었다. 현재의 왕은 왕관의 힘을 폭주시키며 성 전체를 괴물과 병사로 뒤덮었다."),
      p("플레이어는 반란군의 마지막 생존자다. 성에 잠입해 옥좌까지 도달하는 것이 목표지만 — 왕관의 힘 앞에서 결국 처형당한다. 그것이 이 런의 끝이다. 다음 런은 다음 침입이다."),
      p("왕이 되려는 이야기가 아니다. 왕관의 계승을 끝내려는 이야기다."),
      spacer(),

      h2("1.2 컨셉 한 줄"),
      p('"내 유물 조합이 게임 규칙 자체를 바꾼다" — 직업 없는 로그라이크 덱빌딩.'),
      spacer(),

      h2("1.3 핵심 키워드"),
      bullet("턴제 로그라이크 덱빌딩"),
      bullet("유물 기반 빌드 (직업 없음)"),
      bullet("태그 기반 유물 + 규칙 변형 유물의 공존"),
      bullet("카드는 공용, 유물이 게임 규칙을 바꿈"),
      bullet("플레이어가 직접 시너지를 발견하는 구조"),
      bullet("영구 해금 + 코스트 기반 시작 조합"),
      spacer(),

      h2("1.4 타깃 플레이타임"),
      bullet("1판: 10~20분"),
      bullet("총 전투 수: 8~12회"),
      bullet("반복 플레이: 유물 해금을 통한 메타 성장"),
      spacer(),

      h2("1.5 차별점"),
      p("Slay the Spire 계열과 달리 직업이 없고, 카드 풀은 모두 공용이다. 유물은 두 종류로 나뉜다: 특정 태그 카드를 강화하는 태그 기반 유물, 그리고 에너지·드로우·피해 규칙 자체를 바꾸는 규칙 변형 유물이다."),
      p("두 종류의 유물이 공존하며, 플레이어는 조합 시너지를 직접 발견한다. \"집중의 부적\"(충전 카드 누적 2배)과 \"폭발의 도가니\"([충전] 사용 시 화상 부여)를 같이 들면 폭발적 화상 빌드가 된다는 것을 게임이 알려주지 않는다."),
      spacer(),

      // 2. 핵심 게임플레이 루프
      h1("2. 핵심 게임플레이 루프"),

      h2("2.1 메타 루프 (실행 단위)"),
      p("플레이 → 재화 획득 → 새 유물 해금/구매 → 시작 유물 조합 변경 → 다시 플레이"),
      spacer(),

      h2("2.2 1판 루프 (런 단위)"),
      bullet("코스트 한도 내에서 시작 유물 선택"),
      bullet("맵 진행 (전투 / 엘리트 / 이벤트 / 상점 / 휴식)"),
      bullet("전투 승리 시 카드 또는 일회성 유물 보상"),
      bullet("보스 처치 시 클리어, 사망 시 종료"),
      spacer(),

      h2("2.3 1전투 루프 (턴 단위)"),
      bullet("매 턴 카드 N장 드로우, 에너지 회복"),
      bullet("플레이어가 카드 사용 (유물 효과 적용, 키워드 발동)"),
      bullet("적의 의도(intent) 표시 → 턴 종료 → 적 행동"),
      bullet("적 전멸 시 승리"),
      spacer(),

      // 3. 시스템 상세
      h1("3. 시스템 상세"),

      h2("3.1 카드 시스템"),
      p("모든 카드는 공용 풀에서 등장한다. 카드 자체는 단순하게 설계하고, 유물이 카드의 동작을 변화시키는 구조다."),
      spacer(),

      h3("3.1.1 카드 설계 원칙"),
      bullet("기본 카드(타격, 방어)는 시작 덱에 포함"),
      bullet("모든 카드는 1개 이상의 효과 태그를 가진다"),
      bullet("카드 설명에는 결과 수치만 표시, 내부 계수 노출 안 함"),
      bullet("실시간 계산된 피해/방어량을 카드 위에 표시"),
      spacer(),

      h3("3.1.2 시작 덱 구성"),
      bullet("타격 ×4"),
      bullet("방어 ×4"),
      bullet("드로우 카드 ×1 (집중)"),
      bullet("회피 또는 유틸 카드 ×1"),
      spacer(),

      h3("3.1.3 카드 태그 시스템"),
      p("카드는 효과 태그(어떤 효과를 내는가) 1개 이상과 속성 키워드(어떻게 동작하는가) 0~1개의 조합으로 구성된다."),
      spacer(),

      p("효과 태그 목록:"),
      makeTable(
        ["태그", "의미"],
        [
          ["[공격]", "적에게 직접 피해를 주는 카드"],
          ["[방어]", "방어도·회피·피해 감소 관련 카드"],
          ["[연타]", "여러 번 히트하는 공격 카드"],
          ["[화염]", "화상 상태이상 부여 또는 강화 카드"],
          ["[유틸]", "드로우·에너지·덱 조작 등 보조 카드"],
        ],
        [2200, 7160]
      ),
      spacer(),
      p("화상 메커니즘: 중첩 수만큼 매 턴 피해 발생. 턴 종료 시 중첩 -1 감소. 중첩 0이 되면 해제."),
      spacer(),

      p("속성 키워드 목록:"),
      makeTable(
        ["키워드", "의미"],
        [
          ["[소모]", "사용 후 이번 전투동안 덱에서 제거"],
          ["[선천]", "매 전투 시작 시 무조건 손패에 포함"],
          ["[유지]", "턴 종료 시 버리지 않고 손패에 유지"],
          ["[귀환]", "사용 후 버리는 더미 대신 다음 턴 손패로 돌아옴"],
          ["[연쇄]", "사용 시 덱 맨 위 카드 즉시 드로우"],
          ["[충전]", "손패에 있는 동안 매 턴 효과 수치 +N 누적, 사용 시 누적값으로 발동 (사용 후 초기화)"],
          ["[과부하]", "강한 효과 발동, 대신 다음 턴 에너지 -1"],
        ],
        [1800, 7560]
      ),
      spacer(),
      p("조합 예시: 화염검 = [공격][화염] / 집중 = [유틸][선천] / 폭탄 = [화염][소모] / 분노 = [공격][과부하] / 축적의 검 = [공격][충전]"),
      spacer(),

      h2("3.2 유물 시스템 (핵심)"),
      p("유물은 게임의 정체성을 담당하는 시스템이다. 영구 해금형과 일회성 두 종류로 분리한다. 각 종류 안에서 유물은 다시 태그 기반 / 키워드 시너지 / 규칙 변형 세 유형으로 나뉜다. 플레이어가 조합 시너지를 직접 발견하는 것이 핵심 재미다."),
      spacer(),

      h3("3.2.1 영구 유물 (Loadout Relic)"),
      bullet("게임 시작 전 코스트 한도 내에서 자유롭게 조합"),
      bullet("재화를 모아 상점에서 해금/구매"),
      bullet("시작 코스트 한도: 총 5~6 (메타 진행에 따라 점진적 확장 고려)"),
      spacer(),

      p("태그 기반 영구 유물:"),
      makeTable(
        ["유물명", "효과", "코스트"],
        [
          ["대검", "[공격] 카드 피해 +3", "2"],
          ["단검", "[연타] 카드 타수 +1", "2"],
          ["화염 반지", "[화염] 카드 화상 중첩 +2", "2"],
          ["두꺼운 방패", "[방어] 카드 방어도 +4", "2"],
          ["마법사의 책", "[유틸] 카드 사용 시 1장 추가 드로우", "3"],
        ],
        [2200, 5560, 1600]
      ),
      spacer(),

      p("키워드 시너지 영구 유물:"),
      makeTable(
        ["유물명", "효과", "코스트", "키워드"],
        [
          ["집중의 부적", "[충전] 카드 누적 속도 2배 (매 턴 +2N)", "2", "[충전]"],
          ["인내의 시계", "[충전] 카드 사용 후 누적값 50% 유지 (초기화 안 됨)", "3", "[충전]"],
          ["에너지 결정", "[과부하] 카드의 다음 턴 에너지 페널티 제거", "3", "[과부하]"],
          ["폭발의 반지", "[과부하] 카드 효과 +50%", "2", "[과부하]"],
          ["무한의 검", "[귀환] 카드가 돌아올 때마다 효과 영구 +1 누적", "3", "[귀환]"],
          ["선제의 반지", "매 전투 첫 번째 카드 에너지 0", "2", "[선천]"],
        ],
        [2000, 4160, 1200, 2000]
      ),
      spacer(),

      p("규칙 변형 영구 유물:"),
      makeTable(
        ["유물명", "효과", "코스트"],
        [
          ["인내의 망토", "턴 종료 시 손패 남은 카드 수 × 3 방어도 획득", "2"],
          ["저주의 심장", "매 턴 HP -2, 에너지 +1", "1"],
          ["약자의 분노", "HP 30% 이하일 때 모든 카드 효과 2배", "3"],
          ["광기의 왕관 파편", "[방어] 카드 사용 불가, 대신 [공격] 카드 피해 +6", "1"],
        ],
        [2200, 5560, 1600]
      ),
      spacer(),

      h3("3.2.2 일회성 유물 (Run Relic)"),
      bullet("런 도중 엘리트, 이벤트, 보스에서 획득"),
      bullet("이번 런에만 적용, 다음 런으로 이월 없음"),
      bullet("강한 효과 허용 — 빌드를 폭발시키는 역할"),
      spacer(),

      p("태그 기반 일회성 유물:"),
      makeTable(
        ["유물명", "효과"],
        [
          ["강철 팔찌", "[연타] 카드 피해 +2"],
          ["불꽃 코어", "[화염] 카드 즉시 피해 +현재 화상 중첩 수"],
          ["용의 숨결", "[화염] 카드 화상 중첩 2배 부여"],
        ],
        [2800, 6560]
      ),
      spacer(),

      p("키워드 시너지 일회성 유물:"),
      makeTable(
        ["유물명", "효과", "키워드"],
        [
          ["폭발의 도가니", "[충전] 카드 사용 시 누적값만큼 추가로 화상 부여", "[충전][화염]"],
          ["역류의 석판", "[과부하] 카드 사용 직후 에너지 1 즉시 회복", "[과부하]"],
          ["소각의 두루마리", "[소모] 카드 사용 시 덱 맨 위 카드 즉시 발동", "[소모]"],
          ["순환의 고리", "[귀환] 카드 사용 시 손패 다른 카드 1장 추가 드로우", "[귀환]"],
        ],
        [2200, 4560, 2600]
      ),
      spacer(),

      p("규칙 변형 일회성 유물:"),
      makeTable(
        ["유물명", "효과"],
        [
          ["반복의 서", "같은 카드를 쓸수록 효과 +1 누적 (전투 후 초기화)"],
          ["처형인의 도끼", "방어도 없는 적에게 피해 2배"],
          ["집념의 검", "같은 카드 한 턴 2번 사용 시 3번째는 에너지 0"],
          ["불사의 파편", "HP 0 대신 1로 버팀 (1회)"],
        ],
        [2800, 6560]
      ),
      spacer(),

      h2("3.3 적 / 보스"),
      bullet("적 5종, 엘리트 2종, 보스 1종"),
      bullet("각 적은 의도(intent) 시스템으로 다음 행동을 미리 표시"),
      bullet("패턴은 2~3개씩, 단순하지만 유물 빌드에 따라 대응이 달라지도록 설계"),
      bullet("예: 다단히트 적 → 연타 빌드 카운터 / 화염 빌드 유리"),
      spacer(),

      h2("3.4 맵 / 진행 구조"),
      bullet("Slay the Spire식 노드 맵 (단일 맵, 분기 선택)"),
      bullet("총 8~12 전투 + 보스"),
      bullet("노드 종류: 일반 전투 / 엘리트 / 이벤트 / 상점 / 휴식"),
      spacer(),

      h2("3.5 보상 구조"),
      bullet("일반 전투 → 카드 3장 중 1장 선택"),
      bullet("엘리트 → 일회성 유물 + 카드"),
      bullet("이벤트 → 특수 효과 (저주받은 대장간, 모닥불, 보물상자 등)"),
      bullet("상점 → 카드/유물/카드 강화 구매"),
      bullet("보스 → 강력한 일회성 유물 + 메타 재화"),
      spacer(),

      h2("3.6 메타 진행 / 영구 해금"),
      bullet("런 종료 시 재화 획득 (승패 무관, 진행도에 비례)"),
      bullet("재화로 메타 상점에서 영구 유물 구매"),
      bullet("보스 처치 시 새 영구 유물 해금"),
      bullet("해금과 구매를 분리 — 해금만 되어도 구매 전까지는 사용 불가"),
      spacer(),

      // 4. UX / UI 원칙
      h1("4. UX / UI 원칙"),

      h2("4.1 카드 표시"),
      bullet("계수, 내부 수식은 노출하지 않음"),
      bullet("현재 상태 기준 결과 수치를 카드에 실시간 표시"),
      bullet("유물 효과로 강화된 카드는 색상/테두리로 강조"),
      bullet("효과 태그는 아이콘으로 표시 (텍스트 최소화)"),
      bullet("속성 키워드([소모][선천][충전] 등)는 카드 하단에 별도 표시"),
      spacer(),

      h2("4.2 유물 표시"),
      bullet("화면 상단에 현재 보유 유물 아이콘 가로 배치"),
      bullet("마우스 오버 시 효과 설명 툴팁"),
      bullet("발동된 유물은 짧은 시각 피드백 (반짝임, 작은 팝업)"),
      spacer(),

      h2("4.3 적 의도"),
      bullet("적 위에 다음 행동 아이콘 + 예상 수치 표시"),
      bullet("플레이어의 방어/회피 효과가 반영된 예상 피해량 표시"),
      spacer(),

      // 5. 개발 일정
      h1("5. 개발 일정 (3주, 월~금)"),

      h2("5.1 우선순위 원칙"),
      bullet("핵심 루프(전투 → 보상 → 다음 전투) 1주차 안에 동작해야 함"),
      bullet("유물 시스템 (태그 기반 + 규칙 변형 + 키워드 시너지)은 2주차 핵심 작업"),
      bullet("3주차에는 새 기능 추가 금지, 폴리싱과 빌드만"),
      bullet("에셋 제작: Leonardo.ai로 스프라이트 생성 → Unity Sprite Editor로 애니메이션 프레임 구성"),
      bullet("캐릭터/적 애니메이션은 아이들·공격·피격 2~3프레임으로 제한"),
      spacer(),

      h2("5.2 주간 상세 일정"),

      h3("1주차 — 핵심 전투 루프"),
      makeTable(
        ["요일", "작업", "산출물"],
        [
          ["월", "ScriptableObject 카드 데이터 구조 설계, 기본 카드 10장 입력", "카드 데이터 완성"],
          ["화", "전투 씬: 드로우 / 에너지 / 카드 사용 로직 + 카드 드로우·사용·버리기 애니메이션 (DOTween)", "카드 플레이 동작"],
          ["수", "적 시스템: HP, 의도 표시, 적 행동 + Leonardo.ai 스프라이트 생성 및 Animator 세팅 (아이들·공격·피격)", "적 동작 완성"],
          ["목", "승패 판정, 카드 보상 화면", "전투 루프 완성"],
          ["금", "맵 노드 이동 연결, 1주차 빌드 정리", "1주차 플레이 가능 빌드"],
        ],
        [720, 5840, 2800]
      ),
      p("1주차 빌드: 카드 10장으로 전투 → 보상 → 다음 노드 이동까지 동작. 애니메이션 포함."),
      spacer(),

      h3("2주차 — 유물 + 키워드 시스템"),
      makeTable(
        ["요일", "작업", "산출물"],
        [
          ["월", "유물 데이터 구조 (ScriptableObject), 로드아웃 선택 화면", "로드아웃 UI 완성"],
          ["화", "태그 기반 유물 전체 구현 ([공격][방어][연타][화염][유틸])", "태그 유물 동작"],
          ["수", "키워드 전체 구현 ([충전][과부하][귀환][선천][유지][소모][연쇄])", "키워드 동작"],
          ["목", "키워드 시너지 유물 + 규칙 변형 유물, 엘리트/보스 추가", "전체 유물 동작"],
          ["금", "키워드-유물 조합 버그 테스트, 2주차 빌드 정리", "2주차 플레이 가능 빌드"],
        ],
        [720, 5840, 2800]
      ),
      p("2주차 빌드: 로드아웃 선택 → 유물·키워드 전부 동작 → 보스 처치까지 가능."),
      spacer(),

      h3("3주차 — 폴리싱 + 완성"),
      makeTable(
        ["요일", "작업", "산출물"],
        [
          ["월", "메타 진행: 재화 획득, 영구 유물 해금 화면", "메타 루프 완성"],
          ["화", "이벤트 노드 3종 (저주받은 대장간, 모닥불, 보물상자)", "이벤트 동작"],
          ["수", "사운드 (무료 에셋), UI 애니메이션 (유물 발동 반짝임, 피해 숫자 팝업)", "사운드·이펙트 완성"],
          ["목", "UI 폴리싱, 밸런싱 조정", "게임 완성도 향상"],
          ["금", "버그 수정, 빌드 추출, 최종 빌드 정리", "최종 배포 빌드"],
        ],
        [720, 5840, 2800]
      ),
      p("최종 빌드: 메인 메뉴 → 로드아웃 → 런 → 결과 화면. 배포 가능."),
      spacer(),

      // 6. 제외 항목
      h1("6. 제외 항목 (스코프 보호)"),
      p("3주 일정 보호를 위해 다음은 명시적으로 제외한다."),
      bullet("직업 / 클래스 시스템"),
      bullet("스토리 / 컷씬"),
      bullet("멀티플레이 / 온라인 랭킹"),
      bullet("프로시저럴 맵 생성 고도화"),
      bullet("이벤트 30개 이상, 카드 50장 이상 등 콘텐츠 폭주"),
      bullet("3D / 복잡한 애니메이션 (2~3프레임 스프라이트 애니메이션으로 대체)"),
      bullet("자체 사운드 제작 (무료 에셋 사용)"),
      bullet("자체 스프라이트 수작업 제작 (Leonardo.ai로 대체)"),
      spacer(),

      // 7. 리스크
      h1("7. 주요 리스크 및 대응"),

      h2("7.1 밸런싱 시간 부족"),
      p("대응: 카드/유물 수치를 ScriptableObject로 관리해 빌드 없이 수정 가능하도록 구성. 디버그 콘솔로 빠른 테스트."),
      spacer(),

      h2("7.2 유물 효과 구현 복잡도"),
      p("유물이 카드 동작을 변경하는 구조는 잘못 짜면 if-else 지옥이 됨."),
      p("대응: if-else 기반 직접 처리로 단순하게 유지. 유물 수를 15개 이내로 제한해 복잡도 관리. 각 유물의 발동 조건을 명확한 enum으로 정의."),
      spacer(),

      h2("7.3 키워드 상호작용 버그"),
      p("[충전]+[귀환], [과부하]+규칙 변형 유물 등 키워드와 유물의 조합에서 예상치 못한 상호작용이 발생할 수 있음."),
      p("대응: 키워드별 처리 순서를 명확히 정의 (충전 누적 → 과부하 효과 → 유물 트리거 순). 2주차에 조합 테스트 집중."),
      spacer(),

      h2("7.4 UI 정보량 과다"),
      p("카드 + 유물 + 적 의도 + 키워드가 한 화면에 표시됨."),
      p("대응: 계수 숨기기, 결과값 중심, 아이콘화로 정보 밀도 조정."),
    ],
  }],
});

Packer.toBuffer(doc).then((buffer) => {
  writeFileSync("Docs/GDD_v0.3.docx", buffer);
  console.log("GDD_v0.3.docx 생성 완료");
});
