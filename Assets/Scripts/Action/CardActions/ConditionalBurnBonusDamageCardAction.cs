using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn Bonus Damage")]
public class ConditionalBurnBonusDamageCardAction : CardAction {
	public int amount;
	public int bonusAmount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "ConditionalBurnBonusDamageCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("@", amount.ToString())
			.Replace("#", bonusAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		int calculated = CalculateAmountWithContext(context);
		string amountText = calculated > amount ? GetGreenText(calculated.ToString())
			: calculated < amount ? GetRedText(calculated.ToString())
			: calculated.ToString();
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("@", amountText)
			.Replace("#", bonusAmount.ToString());
	}

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		int damage = CalculateAmountWithContext(context);
		if (context.target.HasStatus<Burn>()) damage += bonusAmount;
		context.target.GetDamage(damage);
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.user.CalculateAttackingDamage(result);
		if (context.target != null) { result = context.target.CalculateGainingDamage(result); }
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}
}
