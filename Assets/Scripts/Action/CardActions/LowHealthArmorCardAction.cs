using UnityEngine;

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
		string lowHealthArmorText = FormatPreviewAmount(
			CalculateArmorAmountWithContext(context, lowHealthArmor, CalculationMode.Preview),
			lowHealthArmor
		);
		string normalArmorText = FormatPreviewAmount(
			CalculateArmorAmountWithContext(context, normalArmor, CalculationMode.Preview),
			normalArmor
		);
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", lowHealthArmorText)
			.Replace("#", normalArmorText);
	}

	public override void Execute(CardUseContext context) {
		context.user.AddBlock(ApplyAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int baseAmount = IsLowHealth(context) ? lowHealthArmor : normalArmor;
		return CalculateArmorAmountWithContext(context, baseAmount, mode);
	}

	private int CalculateArmorAmountWithContext(CardUseContext context, int baseAmount, CalculationMode mode) {
		int result = baseAmount;
		result = CalculateGainingArmorModifiers(context.user, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}

	private bool IsLowHealth(CardUseContext context) {
		return context.user.CurrentHealth * 100 <= context.user.MaxHealth * thresholdPercent;
	}
}
