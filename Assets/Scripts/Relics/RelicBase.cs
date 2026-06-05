using System;
using System.Collections.Generic;
using UnityEngine;

// 모든 유물의 기반 클래스.
// 기본값은 아무 효과도 없게 두고, 각 유물은 필요한 훅만 override한다.
public abstract class RelicBase {
	public abstract string relicId { get; }
	public abstract int cost { get; }
	public abstract int effectAmount { get; }
	public virtual CardTag? affectedTag => null;
	public virtual string iconName => GetType().Name;
	public abstract RelicRarity rarity { get; }

	public string displayName => StringTableManager.StringTable[$"{GetType().Name}Name"];

	public string effectDescription {
		get {
			string template = StringTableManager.StringTable[$"{GetType().Name}Desc"]
				.Replace("-", effectAmount.ToString());
			if (!affectedTag.HasValue) return template;

			string tag = StringTableManager.StringTable[affectedTag.Value.ToString()];
			return $"[{tag}] {template}";
		}
	}

	public Sprite icon => Resources.Load<Sprite>($"Sprites/Relics/{iconName}");

	public virtual RelicBase CreateRuntimeCopy() {
		return (RelicBase)Activator.CreateInstance(GetType());
	}

	public virtual void OnTurnStart() { }
	public virtual void OnTurnEnd() { }
	public virtual void OnBattleStart(BattleManager battleManager) { }
	public virtual void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) { }
	public virtual void OnPlayerTurnEnd(BattleManager battleManager, int turnNumber) { }
	public virtual void OnBeforeCardUse(CardUseContext context) { }
	public virtual void OnAfterCardUse(CardUseContext context) { }
	public virtual void OnCardDrawn(BattleManager battleManager, CardInstance card, CardDrawSource source) { }
	public virtual void OnReturnedCardToHand(BattleManager battleManager, CardInstance card) { }
	public virtual int ModifySelfDamage(CardUseContext context, int amount) { return amount; }
	public virtual int ModifyHeal(CardUseContext context, CardAction action, int amount) { return amount; }
	public virtual int ModifyIncomingDamage(DamageContext context, int amount) { return amount; }
	public virtual int ModifyRewardCardCount(MapNodeType nodeType, int count) { return count; }
	public virtual void ModifyRewardCards(MapNodeType nodeType, List<CardInstance> cards) { }
	public virtual int ModifyGoldReward(MapNodeType nodeType, int amount) { return amount; }
	public virtual void OnEnemyKilled(DamageContext context, CharacterBase enemy) { }
	public virtual void OnAfterOwnerDamaged(CharacterBase owner, CharacterBase attacker, int damageTaken) { }
	public virtual bool TryPreventOwnerDeath(CharacterBase owner) { return false; }
	public virtual bool ConsumeSkipEnemyTurn() { return false; }

	public virtual int CalculateAmount(CardAction action, CardInstance instance, int amount) { return amount; }
	public virtual int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		return CalculateAmount(action, context.cardInfo, amount);
	}

	public virtual int CalculateRepeat(CardAction action, CardInstance instance, int repeat) { return repeat; }
	public virtual int CalculateRepeat(CardAction action, CardUseContext context, int repeat) {
		return CalculateRepeat(action, context.cardInfo, repeat);
	}
}
