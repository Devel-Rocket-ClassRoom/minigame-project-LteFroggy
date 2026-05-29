using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Add Burn Stacks")]
public class AddBurnStacksCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "AddBurnStacksCardText";

	public override void Execute(CardUseContext context) {
		if (context.target == null) return;
		if (!context.target.HasStatus<Burn>()) return;
		var burn = new Burn();
		burn.Init(context.target, CalculateAmountWithContext(context), 0);
		context.target.AddStatus(burn);
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}
}
