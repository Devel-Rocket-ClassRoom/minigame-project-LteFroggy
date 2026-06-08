using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn")]
public class ConditionalBurnCardAction : CardAction {
	public int amount;
	public int bonusAmount;
	public override bool IsBurnAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "ConditionalBurnCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", amount.ToString())
			.Replace("#", bonusAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null) return;

		int burnAmount = ApplyAmountWithContext(context);
		if (context.target.HasStatus<Burn>()) burnAmount += bonusAmount;
		var burn = new Burn();
		burn.Init(context.target, burnAmount, 0);
		context.target.AddStatus(burn);
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		result = CalculateGivingBurnModifiers(context.user, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}
