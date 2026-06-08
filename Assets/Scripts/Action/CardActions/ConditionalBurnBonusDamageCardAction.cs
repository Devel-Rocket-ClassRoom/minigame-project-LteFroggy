using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn Bonus Damage")]
public class ConditionalBurnBonusDamageCardAction : CardAction {
	public int amount;
	public int bonusAmount;
	public override bool IsDamageAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "ConditionalBurnBonusDamageCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("#", bonusAmount.ToString())
			.Replace("-", amount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		int calculated = CalculatePreviewAmountWithContext(context);
		string amountText = calculated > amount ? GetGreenText(calculated.ToString())
			: calculated < amount ? GetRedText(calculated.ToString())
			: calculated.ToString();
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("#", bonusAmount.ToString())
			.Replace("-", amountText);
	}

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		int damage = ApplyAmountWithContext(context);
		if (context.target.HasStatus<Burn>()) damage += bonusAmount;
		context.DealDamage(context.target, this, damage);
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) { result = CalculateTakingDamageModifiers(context.target, result, mode); }
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		return result;
	}
}
