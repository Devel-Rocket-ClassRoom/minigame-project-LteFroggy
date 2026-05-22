using System.Collections.Generic;

public class RelicManager : BattleSystemManager {
	private List<RelicBase> _relics = new();
	
	// PlayerData의 RelicList에서 정보 가져오고, 렌더링
	public override void StartBattle() {
		base.StartBattle();
		_relics = PlayerData.Instance.Relics;
	}
	
	public int CalculateAmountWithRelics(CardInstance card, CardAction action, int amount) {
		// 유물 기반으로 실제 들어가는 값을 계산한다.
		foreach (var relic in _relics) { amount = relic.CalculateAmount(action, card, amount); }
		return amount;
	}
	
	public int CalculateRepeatWithRelics(CardInstance card, CardAction action, int repeat) {
		foreach (var relic in _relics) { repeat = relic.CalculateRepeat(action, card, repeat); }
		return repeat;
	}
}