using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Give Burn")]
public class BurnCardAction : CardAction {
	public int amount;
	public override bool IsBurnAction => true;
	protected override int Amount => amount;
	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		result = CalculateGivingBurnModifiers(context.user, result, mode);
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);

		return result;
	}

	public override void Execute(CardUseContext context) {
		var burn = new Burn();
		burn.Init(context.target, ApplyAmountWithContext(context), 0);
		context.target.AddStatus(burn);
	}

	public override string CardDescriptionKey => "BurnCardText";
}
