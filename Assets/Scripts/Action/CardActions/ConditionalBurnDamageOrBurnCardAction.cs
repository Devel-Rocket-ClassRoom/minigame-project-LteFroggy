using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Conditional Burn Damage Or Burn")]
public class ConditionalBurnDamageOrBurnCardAction : CardAction {
	public int damageAmount;
	public int burnAmount;
	public override bool IsDamageAction => true;
	public override bool IsBurnAction => true;

	protected override int Amount => damageAmount;
	public override string CardDescriptionKey => "ConditionalBurnDamageOrBurnCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", damageAmount.ToString())
			.Replace("#", burnAmount.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		if (context.target.HasStatus<Burn>()) {
			context.DealDamage(context.target, this, ApplyAmountWithContext(context));
			context.user.PlayAttackAnimation();
			return;
		}

		var burn = new Burn();
		int result = CalculateGivingBurnModifiers(context.user, burnAmount, CalculationMode.Apply);
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		burn.Init(context.target, result, 0);
		context.target.AddStatus(burn);
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = damageAmount;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) result = CalculateTakingDamageModifiers(context.target, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}
