using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Heal Player")]
public class HealPlayerCardAction : CardAction {
	public int amount;
	public override bool IsHealAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "HealPlayerCardText";

	public override void Execute(CardUseContext context) {
		context.HealPlayer(this, ApplyAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}
