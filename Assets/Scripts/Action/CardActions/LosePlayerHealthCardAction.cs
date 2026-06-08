using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Lose Player Health")]
public class LosePlayerHealthCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "LosePlayerHealthCardText";

	public override void Execute(CardUseContext context) {
		context.DamagePlayer(this, ApplyAmountWithContext(context));
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
}
