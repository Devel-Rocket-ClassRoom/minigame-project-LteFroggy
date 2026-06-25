using System.Collections.Generic;

public class RelicManager : BattleSystemManager {
	private IReadOnlyList<RelicBase> _relics = new List<RelicBase>();

	public override void StartBattle() {
		base.StartBattle();
		_relics = GamePlayData.Instance.Relics;
	}

	public void OnBattleStart(BattleManager battleManager) {
		foreach (var relic in _relics) relic.OnBattleStart(battleManager);
	}

	public void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		foreach (var relic in _relics) relic.OnPlayerTurnStart(battleManager, turnNumber);
	}

	public void OnPlayerTurnEnd(BattleManager battleManager, int turnNumber) {
		foreach (var relic in _relics) relic.OnPlayerTurnEnd(battleManager, turnNumber);
	}

	public void OnBeforeCardUse(CardUseContext context) {
		foreach (var relic in _relics) relic.OnBeforeCardUse(context);
	}

	public void OnPreviewCardUse(CardUseContext context) {
		foreach (var relic in _relics) relic.OnPreviewCardUse(context);
	}

	public void OnAfterCardUse(CardUseContext context) {
		foreach (var relic in _relics) relic.OnAfterCardUse(context);
	}

	public void OnCardDrawn(BattleManager battleManager, CardInstance card, CardDrawSource source) {
		foreach (var relic in _relics) relic.OnCardDrawn(battleManager, card, source);
	}

	public void OnReturnedCardToHand(BattleManager battleManager, CardInstance card) {
		foreach (var relic in _relics) relic.OnReturnedCardToHand(battleManager, card);
	}

	public int ModifySelfDamage(CardUseContext context, int amount) {
		foreach (var relic in _relics) amount = relic.ModifySelfDamage(context, amount);
		return amount;
	}

	public int ModifyHeal(CardUseContext context, CardAction action, int amount) {
		foreach (var relic in _relics) amount = relic.ModifyHeal(context, action, amount);
		return amount;
	}

	public int ModifyIncomingDamage(DamageContext context, int amount) {
		foreach (var relic in _relics) amount = relic.ModifyIncomingDamage(context, amount);
		return amount;
	}

	public int ModifyRewardCardCount(MapNodeType nodeType, int count) {
		foreach (var relic in _relics) count = relic.ModifyRewardCardCount(nodeType, count);
		return count;
	}

	public void ModifyRewardCards(MapNodeType nodeType, List<CardInstance> cards) {
		foreach (var relic in _relics) relic.ModifyRewardCards(nodeType, cards);
	}

	public int ModifyGoldReward(MapNodeType nodeType, int amount) {
		foreach (var relic in _relics) amount = relic.ModifyGoldReward(nodeType, amount);
		return amount;
	}

	public void OnEnemyKilled(DamageContext context, CharacterBase enemy) {
		if (enemy is not EnemyInstance) return;
		foreach (var relic in _relics) relic.OnEnemyKilled(context, enemy);
	}

	public void OnAfterOwnerDamaged(CharacterBase owner, CharacterBase attacker, int damageTaken) {
		if (damageTaken <= 0) return;
		foreach (var relic in _relics) relic.OnAfterOwnerDamaged(owner, attacker, damageTaken);
	}

	public bool TryPreventOwnerDeath(CharacterBase owner) {
		foreach (var relic in _relics) {
			if (relic.TryPreventOwnerDeath(owner)) return true;
		}
		return false;
	}

	public bool ConsumeSkipEnemyTurn() {
		bool skip = false;
		foreach (var relic in _relics) {
			if (relic.ConsumeSkipEnemyTurn()) skip = true;
		}
		return skip;
	}

	public int CalculateAmountWithRelics(CardUseContext context, CardAction action, int amount) {
		foreach (var relic in _relics) amount = relic.CalculateAmount(action, context, amount);
		amount = UnityEngine.Mathf.RoundToInt(amount * context.AmountMultiplier * context.cardInfo.BattleAmountMultiplier);
		return amount;
	}

	public int CalculateAmountWithRelics(CardInstance card, CardAction action, int amount) {
		foreach (var relic in _relics) amount = relic.CalculateAmount(action, card, amount);
		amount = UnityEngine.Mathf.RoundToInt(amount * card.BattleAmountMultiplier);
		return amount;
	}

	public int CalculateRepeatWithRelics(CardUseContext context, CardAction action, int repeat) {
		foreach (var relic in _relics) repeat = relic.CalculateRepeat(action, context, repeat);
		return repeat;
	}

	public int CalculateRepeatWithRelics(CardInstance card, CardAction action, int repeat) {
		foreach (var relic in _relics) repeat = relic.CalculateRepeat(action, card, repeat);
		return repeat;
	}
}
