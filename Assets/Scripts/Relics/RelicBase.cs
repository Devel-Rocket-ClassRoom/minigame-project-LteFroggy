using UnityEngine;

// 모든 유물의 기반 클래스
// 유물 종류마다 이 클래스를 상속받아 구현한다
//
// [텍스트 표시 방식]
//   displayName      : KorStringData.csv의 "{클래스명}Name" 키로 조회
//   effectDescription: KorStringData.csv의 "{클래스명}Desc" 키에서 @를 effectAmount로 치환,
//                      affectedTag가 있으면 앞에 "[태그명]" 추가
//   예) Greatsword → "[공격] 카드 피해 +2"
public abstract class RelicBase {
	public abstract string relicId { get; }

	// 로드아웃 코스트 (전체 합계가 costLimit 이하여야 선택 가능)
	public abstract int cost { get; }

	// 효과 수치 (CSV 설명의 @ 플레이스홀더를 이 값으로 치환)
	public abstract int effectAmount { get; }

	// 이 유물이 효과를 발동하는 카드 태그 (없으면 null)
	public virtual CardTag? affectedTag => null;

	// StringTable에서 유물 이름을 가져온다
	public string displayName => StringTableManager.StringTable[$"{GetType().Name}Name"];

	// StringTable 템플릿 + effectAmount + affectedTag 조합으로 효과 설명을 조립한다
	public string effectDescription {
		get {
			string template = StringTableManager.StringTable[$"{GetType().Name}Desc"]
				.Replace("@", effectAmount.ToString());
			if (!affectedTag.HasValue) return template;
			string tag = StringTableManager.StringTable[affectedTag.Value.ToString()];
			return $"[{tag}] {template}";
		}
	}

	// Resources/Sprites/Relics/{클래스명} 경로에서 아이콘 로드
	public Sprite icon => Resources.Load<Sprite>($"Sprites/Relics/{GetType().Name}");
	public abstract RelicRarity rarity { get; }

	// 턴 시작/종료 시 호출되는 훅 (필요한 유물만 오버라이드)
	public virtual void OnTurnStart() { }
	public virtual void OnTurnEnd() { }

	// 카드 액션의 수치/횟수를 유물 효과로 보정한다
	// 기본 구현은 그대로 반환, 효과가 있는 유물만 오버라이드
	public virtual int CalculateAmount(CardAction action, CardInstance instance, int amount) { return amount; }
	public virtual int CalculateRepeat(CardAction action, CardInstance instance, int repeat) { return repeat; }
}
