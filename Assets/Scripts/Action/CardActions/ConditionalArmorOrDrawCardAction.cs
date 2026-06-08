using UnityEngine;

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
		string armorText = FormatPreviewAmount(CalculatePreviewAmountWithContext(context), armorAmount);
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", armorText)
			.Replace("#", drawAmount.ToString());
	}

	public override void Execute(CardUseContext context) {
		if (context.user.Block == 0) {
			context.user.AddBlock(ApplyAmountWithContext(context));
			context.user.PlaySkillAnimation();
		} else {
			context.DrawCards(drawAmount);
		}
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = armorAmount;
		result = CalculateGainingArmorModifiers(context.user, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}
