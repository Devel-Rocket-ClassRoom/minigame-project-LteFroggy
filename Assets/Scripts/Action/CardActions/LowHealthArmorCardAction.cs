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
