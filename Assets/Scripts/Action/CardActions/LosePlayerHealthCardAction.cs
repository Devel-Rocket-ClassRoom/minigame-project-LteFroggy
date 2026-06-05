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
