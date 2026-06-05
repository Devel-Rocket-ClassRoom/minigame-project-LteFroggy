using System.Collections.Generic;
using UnityEngine;

public abstract class AdditionalRelicBase : RelicBase {
	public override string iconName => GetType().Name;
	protected static bool IsPlayer(CharacterBase character) => character is PlayerCharacter;

	protected static void AddWeakness(CharacterBase target, int amount) {
		if (target == null || target.IsDead) return;
		var weakness = new Weakness();
		weakness.Init(target, 0, amount);
		target.AddStatus(weakness);
	}

	protected static void AddBurn(CharacterBase target, int amount) {
		if (target == null || target.IsDead) return;
		var burn = new Burn();
		burn.Init(target, amount, 0);
		target.AddStatus(burn);
	}

	protected static void AddStrength(CharacterBase target, int amount) {
		if (target == null || target.IsDead) return;
		var strength = new Strength();
		strength.Init(target, amount, 0);
		target.AddStatus(strength);
	}
}

public class SageGlasses : AdditionalRelicBase {
	private bool _usedThisTurn;
	public override string relicId => "3";
	public override int cost => 3;
	public override int effectAmount => 1;
	public override CardTag? affectedTag => CardTag.Util;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		_usedThisTurn = false;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		if (_usedThisTurn || context.cardInfo._cardDefinition.tag != CardTag.Util) return;
		_usedThisTurn = true;
		context.battleManager.CardUseManager.GainEnergy(effectAmount);
	}
}

public class RuneOfReturn : AdditionalRelicBase {
	public override string relicId => "4";
	public override int cost => 2;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnReturnedCardToHand(BattleManager battleManager, CardInstance card) {
		battleManager.Player.AddBlock(effectAmount);
	}
}

public class OathOfIncineration : AdditionalRelicBase {
	public override string relicId => "5";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnAfterCardUse(CardUseContext context) {
		if (!context.cardInfo.Keyword.IsExhaust) return;
		context.battleManager.DeckManager.DrawCard(CardDrawSource.RelicEffect);
	}
}

public class CrownShardOfDesire : AdditionalRelicBase {
	public override string relicId => "6";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override int ModifyRewardCardCount(MapNodeType nodeType, int count) {
		return nodeType is MapNodeType.Battle or MapNodeType.Elite ? count + effectAmount : count;
	}
}

public class OathOfRebel : AdditionalRelicBase {
	public override string relicId => "7";
	public override int cost => 2;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnBattleStart(BattleManager battleManager) {
		var context = new DamageContext(battleManager, battleManager.Player, battleManager.Player, null, null, DamageSourceType.Relic, true, true);
		battleManager.Player.GetDamage(3, context);
		battleManager.DeckManager.AddNextTurnDrawBonus(effectAmount);
	}
}

public class RustedThornArmor : AdditionalRelicBase {
	public override string relicId => "8";
	public override int cost => 2;
	public override int effectAmount => 30;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnAfterOwnerDamaged(CharacterBase owner, CharacterBase attacker, int damageTaken) {
		if (!IsPlayer(owner) || attacker == null || attacker == owner || attacker.IsDead) return;
		int reflectDamage = Mathf.RoundToInt(damageTaken * (effectAmount / 100f));
		var context = new DamageContext(owner.BattleManager, owner, attacker, null, null, DamageSourceType.Relic);
		attacker.GetDamage(reflectDamage, context);
	}
}

public class FrozenChains : AdditionalRelicBase {
	public override string relicId => "9";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override CardTag? affectedTag => CardTag.Defense;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnAfterCardUse(CardUseContext context) {
		if (context.cardInfo._cardDefinition.tag != CardTag.Defense) return;
		AddWeakness(context.target, effectAmount);
	}
}

public class PromiseOfRing : AdditionalRelicBase {
	public override string relicId => "10";
	public override int cost => 3;
	public override int effectAmount => -1;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnCardDrawn(BattleManager battleManager, CardInstance card, CardDrawSource source) {
		if (source != CardDrawSource.CardEffect) return;
		card.AddTemporaryCostModifier(effectAmount);
	}
}

public class HungrySword : AdditionalRelicBase {
	public override string relicId => "11";
	public override int cost => 3;
	public override int effectAmount => 25;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnEnemyKilled(DamageContext context, CharacterBase enemy) {
		if (context.cardContext == null) return;
		var hand = context.cardContext.battleManager.DeckManager.HandPile;
		if (hand.Count == 0) return;
		hand[Random.Range(0, hand.Count)].AddBattleAmountMultiplier(1f + effectAmount / 100f);
	}
}

public class AshGuide : AdditionalRelicBase {
	public override string relicId => "12";
	public override int cost => 3;
	public override int effectAmount => 30;
	public override RelicRarity rarity => RelicRarity.Epic;

	public override void OnEnemyKilled(DamageContext context, CharacterBase enemy) {
		if (context.sourceType != DamageSourceType.Burn || enemy.BattleManager == null) return;
		var player = enemy.BattleManager.Player;
		int lostHealth = player.MaxHealth - player.CurrentHealth;
		player.GetHeal(Mathf.RoundToInt(lostHealth * (effectAmount / 100f)));
	}
}

public class ForgottenBook : AdditionalRelicBase {
	private bool _usedThisTurn;
	public override string relicId => "13";
	public override int cost => 3;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Epic;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		_usedThisTurn = false;
	}

	public override void OnBeforeCardUse(CardUseContext context) {
		if (_usedThisTurn) return;
		_usedThisTurn = true;
		context.AmountMultiplier *= effectAmount;
		context.ForceExhaustAfterUse = true;
	}
}

public class CrownKiss : AdditionalRelicBase {
	public override string relicId => "14";
	public override int cost => 2;
	public override int effectAmount => 50;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		if (!action.IsDamageAction) return amount;
		MapNodeType type = GamePlayData.Instance.InGameMapData.NodeNow.Config.Type;
		return type == MapNodeType.Boss
			? Mathf.RoundToInt(amount * 1.5f)
			: Mathf.RoundToInt(amount * 0.75f);
	}
}

public class ExecutionChain : AdditionalRelicBase {
	private readonly Dictionary<string, int> _useCounts = new();
	public override string relicId => "15";
	public override int cost => 3;
	public override int effectAmount => 25;
	public override RelicRarity rarity => RelicRarity.Epic;

	public override void OnBattleStart(BattleManager battleManager) {
		_useCounts.Clear();
	}

	public override int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		if (!action.IsDamageAction || context.target == null) return amount;
		string key = GetKey(context);
		return _useCounts.TryGetValue(key, out int count)
			? Mathf.RoundToInt(amount * (1f + count * effectAmount / 100f))
			: amount;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		if (context.target == null) return;
		string key = GetKey(context);
		_useCounts[key] = _useCounts.TryGetValue(key, out int count) ? count + 1 : 1;
	}

	private static string GetKey(CardUseContext context) {
		return $"{context.cardInfo._cardDefinition.cardId}:{context.target.GetInstanceID()}";
	}
}

public class GraveBreath : AdditionalRelicBase {
	private bool _used;
	public override string relicId => "16";
	public override int cost => 3;
	public override int effectAmount => 50;
	public override RelicRarity rarity => RelicRarity.Legendary;

	public override bool TryPreventOwnerDeath(CharacterBase owner) {
		if (_used || !IsPlayer(owner)) return false;
		_used = true;
		owner.SetCurrentHealth(Mathf.Max(1, Mathf.RoundToInt(owner.MaxHealth * (effectAmount / 100f))));

		if (owner.BattleManager == null) return true;
		foreach (var card in owner.BattleManager.DeckManager.HandPile) {
			card.Keyword.Add(CardKeywordType.Exhaust);
		}
		return true;
	}
}

public class SandsOfTime : AdditionalRelicBase {
	private bool _skipEnemyTurn;
	public override string relicId => "17";
	public override int cost => 3;
	public override int effectAmount => 5;
	public override RelicRarity rarity => RelicRarity.Legendary;

	public override void OnPlayerTurnEnd(BattleManager battleManager, int turnNumber) {
		if (turnNumber > 0 && turnNumber % effectAmount == 0)
			_skipEnemyTurn = true;
	}

	public override bool ConsumeSkipEnemyTurn() {
		if (!_skipEnemyTurn) return false;
		_skipEnemyTurn = false;
		return true;
	}
}

public class SmallBrazier : AdditionalRelicBase {
	private bool _usedThisBattle;
	public override string relicId => "18";
	public override int cost => 1;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnBattleStart(BattleManager battleManager) {
		_usedThisBattle = false;
	}

	public override int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		return action.IsBurnAction && !_usedThisBattle ? amount + effectAmount : amount;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		if (HasBurnAction(context)) _usedThisBattle = true;
	}

	private static bool HasBurnAction(CardUseContext context) {
		foreach (var action in context.cardInfo._cardDefinition.actions) {
			if (action != null && action.IsBurnAction) return true;
		}
		return false;
	}
}

public class AshCache : AdditionalRelicBase {
	public override string relicId => "19";
	public override int cost => 1;
	public override int effectAmount => 2;
	public override CardTag? affectedTag => CardTag.Fire;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnAfterCardUse(CardUseContext context) {
		if (context.cardInfo._cardDefinition.tag == CardTag.Fire)
			context.user.AddBlock(effectAmount);
	}
}

public class WarDrum : AdditionalRelicBase {
	private bool _usedThisTurn;
	public override string relicId => "20";
	public override int cost => 2;
	public override int effectAmount => 5;
	public override CardTag? affectedTag => CardTag.Attack;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		_usedThisTurn = false;
	}

	public override int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		return !_usedThisTurn && action.IsDamageAction && context.cardInfo._cardDefinition.tag == CardTag.Attack
			? amount + effectAmount
			: amount;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		if (context.cardInfo._cardDefinition.tag == CardTag.Attack)
			_usedThisTurn = true;
	}
}

public class LeadDice : AdditionalRelicBase {
	public override string relicId => "21";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void ModifyRewardCards(MapNodeType nodeType, List<CardInstance> cards) {
		for (int i = 0; i < cards.Count; i++) {
			if (cards[i]._cardDefinition.rarity != CardRarity.Common) continue;
			var excluded = new HashSet<int>();
			foreach (var card in cards) excluded.Add(card._cardDefinition.cardId);
			CardInstance replacement = GamePlayData.Instance.GetRandomRewardCard(CardRarity.Common, excluded);
			if (replacement != null) cards[i] = replacement;
			return;
		}
	}
}

public class OldMap : AdditionalRelicBase {
	public override string relicId => "22";
	public override int cost => 1;
	public override int effectAmount => 5;
	public override RelicRarity rarity => RelicRarity.Common;

	public override int ModifyGoldReward(MapNodeType nodeType, int amount) {
		return nodeType switch {
			MapNodeType.Boss => amount + 15,
			MapNodeType.Battle or MapNodeType.Elite => amount + effectAmount,
			_ => amount,
		};
	}
}

public class BloodstainedBandage : AdditionalRelicBase {
	public override string relicId => "23";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override RelicRarity rarity => RelicRarity.Common;

	public override int ModifySelfDamage(CardUseContext context, int amount) {
		return Mathf.Max(0, amount - effectAmount);
	}
}

public class Whetstone : AdditionalRelicBase {
	public override string relicId => "24";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override CardTag? affectedTag => CardTag.Attack;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnEnemyKilled(DamageContext context, CharacterBase enemy) {
		if (context.cardContext == null || context.cardContext.cardInfo._cardDefinition.tag != CardTag.Attack) return;
		context.cardContext.battleManager.DeckManager.DrawCard(CardDrawSource.RelicEffect);
	}
}

public class HeavyBoots : AdditionalRelicBase {
	private bool _pendingBlock;
	public override string relicId => "25";
	public override int cost => 2;
	public override int effectAmount => 3;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		if (!_pendingBlock) return;
		_pendingBlock = false;
		battleManager.Player.AddBlock(effectAmount);
	}

	public override void OnPlayerTurnEnd(BattleManager battleManager, int turnNumber) {
		foreach (var card in battleManager.DeckManager.HandPile) {
			if (card.Keyword.IsRetain) {
				_pendingBlock = true;
				return;
			}
		}
	}
}

public class DryGunpowder : AdditionalRelicBase {
	public override string relicId => "26";
	public override int cost => 2;
	public override int effectAmount => 25;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override int CalculateAmount(CardAction action, CardUseContext context, int amount) {
		return context.cardInfo.Keyword.IsOverload
			? Mathf.RoundToInt(amount * (1f + effectAmount / 100f))
			: amount;
	}
}

public class ColdHeart : AdditionalRelicBase {
	private bool _usedCardThisTurn;
	private bool _previousTurnWasIdle;
	public override string relicId => "27";
	public override int cost => 2;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		if (_previousTurnWasIdle)
			AddStrength(battleManager.Player, effectAmount);
		_usedCardThisTurn = false;
		_previousTurnWasIdle = false;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		_usedCardThisTurn = true;
	}

	public override void OnPlayerTurnEnd(BattleManager battleManager, int turnNumber) {
		_previousTurnWasIdle = !_usedCardThisTurn;
	}
}

public class WatchmanBell : AdditionalRelicBase {
	private bool _usedThisBattle;
	public override string relicId => "28";
	public override int cost => 1;
	public override int effectAmount => 5;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnBattleStart(BattleManager battleManager) {
		_usedThisBattle = false;
	}

	public override int ModifyIncomingDamage(DamageContext context, int amount) {
		if (_usedThisBattle || !IsPlayer(context.target)) return amount;
		_usedThisBattle = true;
		return Mathf.Max(0, amount - effectAmount);
	}
}

public class QuickHand : AdditionalRelicBase {
	private bool _usedThisTurn;
	public override string relicId => "29";
	public override int cost => 2;
	public override int effectAmount => 1;
	public override RelicRarity rarity => RelicRarity.Common;

	public override void OnPlayerTurnStart(BattleManager battleManager, int turnNumber) {
		_usedThisTurn = false;
	}

	public override void OnAfterCardUse(CardUseContext context) {
		if (_usedThisTurn || context.cardInfo.Cost != 0) return;
		_usedThisTurn = true;
		context.battleManager.DeckManager.DrawCard(CardDrawSource.RelicEffect);
	}
}

public class RoyalEmblem : AdditionalRelicBase {
	public override string relicId => "30";
	public override int cost => 2;
	public override int effectAmount => 20;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override int ModifyGoldReward(MapNodeType nodeType, int amount) {
		return nodeType is MapNodeType.Elite or MapNodeType.Boss ? amount + effectAmount : amount;
	}
}

public class BrokenChalice : AdditionalRelicBase {
	public override string relicId => "31";
	public override int cost => 2;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnBattleStart(BattleManager battleManager) {
		var context = new DamageContext(battleManager, battleManager.Player, battleManager.Player, null, null, DamageSourceType.Relic, true, true);
		battleManager.Player.GetDamage(1, context);
	}

	public override int ModifyHeal(CardUseContext context, CardAction action, int amount) {
		return action.IsHealAction ? amount + effectAmount : amount;
	}
}

public class BlackCandlestick : AdditionalRelicBase {
	public override string relicId => "32";
	public override int cost => 2;
	public override int effectAmount => 2;
	public override RelicRarity rarity => RelicRarity.Rare;

	public override void OnAfterCardUse(CardUseContext context) {
		if (!context.cardInfo.Keyword.IsExhaust || context.target == null) return;
		AddBurn(context.target, effectAmount);
	}
}
