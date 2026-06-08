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
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.user.Block == 0) {
			context.user.AddBlock(CalculateGainingArmorModifiers(context.user, ApplyAmountWithContext(context), CalculationMode.Apply));
			context.user.PlaySkillAnimation();
		} else {
			context.DrawCards(drawAmount);
		}
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, armorAmount);
	}
}
