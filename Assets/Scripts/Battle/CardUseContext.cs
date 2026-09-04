using System.Collections.Generic;

public class CardUseContext : BattleContextBase {
	public readonly List<CharacterBase> targets;
	public readonly CardInstance cardInfo;
	public readonly BattleManager battleManager;
	public readonly RelicManager relicManager;
	public bool ForceExhaustAfterUse { get; set; }
	public float AmountMultiplier { get; set; } = 1f;
	public bool IsPreview { get; set; }
	
	public CardUseContext(BattleManager battleManager, RelicManager relicManager, CharacterBase user, List<CharacterBase> targets, CharacterBase target, CardInstance cardInfo) : base(user, target) {
		this.targets = targets;
		this.battleManager = battleManager;
		this.relicManager = relicManager;
		this.cardInfo = cardInfo;
	}

	public int DealDamage(CharacterBase damageTarget, CardAction action, int amount, DamageSourceType sourceType = DamageSourceType.Card) {
		if (damageTarget == null || damageTarget.IsDead) return 0;

		int before = damageTarget.CurrentHealth;
		var damageContext = new DamageContext(
			battleManager,
			user,
			damageTarget,
			this,
			action,
			sourceType
		);
		damageTarget.GetDamage(amount, damageContext);
		return before - damageTarget.CurrentHealth;
	}

	public int DamagePlayer(CardAction action, int amount) {
		int adjusted = relicManager.ModifySelfDamage(this, amount);
		int before = user.CurrentHealth;
		var damageContext = new DamageContext(
			battleManager,
			user,
			user,
			this,
			action,
			DamageSourceType.SelfDamage,
			true,
			true
		);
		user.GetDamage(adjusted, damageContext);
		return before - user.CurrentHealth;
	}

	public int HealPlayer(CardAction action, int amount) {
		int adjusted = relicManager.ModifyHeal(this, action, amount);
		int before = user.CurrentHealth;
		user.GetHeal(adjusted);
		return user.CurrentHealth - before;
	}

	public void DrawCards(int amount, CardDrawSource source = CardDrawSource.CardEffect) {
		for (int i = 0; i < amount; i++) {
			battleManager.DeckManager.DrawCard(source);
		}
	}
}
