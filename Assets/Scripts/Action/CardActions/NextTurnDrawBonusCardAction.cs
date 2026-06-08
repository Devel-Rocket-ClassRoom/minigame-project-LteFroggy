using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Next Turn Draw Bonus")]
public class NextTurnDrawBonusCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "NextTurnDrawBonusCardText";

	public override void Execute(CardUseContext context) {
		context.battleManager.DeckManager.AddNextTurnDrawBonus(ApplyAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}
