using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Lose Player Health")]
public class LosePlayerHealthCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "LosePlayerHealthCardText";

	public override void Execute(CardUseContext context) {
		context.DamagePlayer(this, CalculateAmountWithContext(context));
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Heal Player")]
public class HealPlayerCardAction : CardAction {
	public int amount;
	public override bool IsHealAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "HealPlayerCardText";

	public override void Execute(CardUseContext context) {
		context.HealPlayer(this, CalculateAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Execute")]
public class ExecuteCardAction : CardAction {
	public int fallbackDamage;
	public int thresholdPercent = 30;
	public override bool IsDamageAction => true;

	protected override int Amount => fallbackDamage;
	public override string CardDescriptionKey => "ExecuteCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", fallbackDamage.ToString())
			.Replace("#", thresholdPercent.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		if (context.target.CurrentHealth * 100 <= context.target.MaxHealth * thresholdPercent) {
			var damageContext = new DamageContext(
				context.battleManager,
				context.user,
				context.target,
				context,
				this,
				DamageSourceType.Execute,
				true
			);
			context.target.GetDamage(context.target.CurrentHealth, damageContext);
		} else {
			context.DealDamage(context.target, this, CalculateAmountWithContext(context));
		}
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = fallbackDamage;
		result = context.user.CalculateAttackingDamage(result);
		if (context.target != null) result = context.target.CalculateGainingDamage(result);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn")]
public class ConditionalBurnCardAction : CardAction {
	public int amount;
	public int bonusAmount;
	public override bool IsBurnAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "ConditionalBurnCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", amount.ToString())
			.Replace("#", bonusAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null) return;

		int burnAmount = CalculateAmountWithContext(context);
		if (context.target.HasStatus<Burn>()) burnAmount += bonusAmount;
		var burn = new Burn();
		burn.Init(context.target, burnAmount, 0);
		context.target.AddStatus(burn);
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.user.CalculateGiveBurn(result);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn Damage Or Burn")]
public class ConditionalBurnDamageOrBurnCardAction : CardAction {
	public int damageAmount;
	public int burnAmount;
	public override bool IsDamageAction => true;
	public override bool IsBurnAction => true;

	protected override int Amount => damageAmount;
	public override string CardDescriptionKey => "ConditionalBurnDamageOrBurnCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", damageAmount.ToString())
			.Replace("#", burnAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		if (context.target.HasStatus<Burn>()) {
			context.DealDamage(context.target, this, CalculateAmountWithContext(context));
			context.user.PlayAttackAnimation();
			return;
		}

		var burn = new Burn();
		int result = context.user.CalculateGiveBurn(burnAmount);
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		burn.Init(context.target, result, 0);
		context.target.AddStatus(burn);
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = damageAmount;
		result = context.user.CalculateAttackingDamage(result);
		if (context.target != null) result = context.target.CalculateGainingDamage(result);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Armor Or Draw")]
public class ConditionalArmorOrDrawCardAction : CardAction {
	public int armorAmount;
	public int drawAmount;
	public override bool IsBlockAction => true;

	protected override int Amount => armorAmount;
	public override string CardDescriptionKey => "ConditionalArmorOrDrawCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", armorAmount.ToString())
			.Replace("#", drawAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.user.Block == 0) {
			context.user.AddBlock(context.user.CalculateGainingArmor(CalculateAmountWithContext(context)));
			context.user.PlaySkillAnimation();
		} else {
			context.DrawCards(drawAmount);
		}
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return context.relicManager.CalculateAmountWithRelics(context, this, armorAmount);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Low Health Armor")]
public class LowHealthArmorCardAction : CardAction {
	public int lowHealthArmor;
	public int normalArmor;
	public int thresholdPercent = 50;
	public override bool IsBlockAction => true;

	protected override int Amount => lowHealthArmor;
	public override string CardDescriptionKey => "LowHealthArmorCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", lowHealthArmor.ToString())
			.Replace("#", normalArmor.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		int baseAmount = context.user.CurrentHealth * 100 <= context.user.MaxHealth * thresholdPercent
			? lowHealthArmor
			: normalArmor;
		int amount = context.relicManager.CalculateAmountWithRelics(context, this, baseAmount);
		context.user.AddBlock(context.user.CalculateGainingArmor(amount));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return lowHealthArmor;
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Next Turn Draw Bonus")]
public class NextTurnDrawBonusCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "NextTurnDrawBonusCardText";

	public override void Execute(CardUseContext context) {
		context.battleManager.DeckManager.AddNextTurnDrawBonus(CalculateAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}

[CreateAssetMenu(menuName = "Card/Card Actions/Lost Health Bonus Damage")]
public class LostHealthBonusDamageCardAction : CardAction {
	public int baseDamage;
	public int lostHealthPercent = 20;
	public override bool IsDamageAction => true;

	protected override int Amount => baseDamage;
	public override string CardDescriptionKey => "LostHealthBonusDamageCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", baseDamage.ToString())
			.Replace("#", lostHealthPercent.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		int lostHealth = context.user.MaxHealth - context.user.CurrentHealth;
		int damage = baseDamage + Mathf.RoundToInt(lostHealth * (lostHealthPercent / 100f));
		damage = context.user.CalculateAttackingDamage(damage);
		damage = context.target.CalculateGainingDamage(damage);
		damage = context.relicManager.CalculateAmountWithRelics(context, this, damage);
		context.DealDamage(context.target, this, damage);
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return baseDamage;
	}
}
