using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Execute")]
public class ExecuteCardAction : CardAction {
	public int fallbackDamage;
	public int thresholdPercent = 30;
	public override bool IsDamageAction => true;

	protected override int Amount => fallbackDamage;
	public override string CardDescriptionKey => "ExecuteCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", fallbackDamage.ToString())
			.Replace("#", thresholdPercent.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		string damageText = FormatPreviewAmount(CalculatePreviewAmountWithContext(context), fallbackDamage);
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", damageText)
			.Replace("#", thresholdPercent.ToString());
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		if (context.target.CurrentHealth * 100 <= context.target.MaxHealth * thresholdPercent) {
			var damageContext = new DamageContext(
				context.battleManager,
				context.user,
				context.target,
				context,
				this,
				DamageSourceType.Execute,
				true
			);
			context.target.GetDamage(context.target.CurrentHealth, damageContext);
		} else {
			context.DealDamage(context.target, this, ApplyAmountWithContext(context));
		}
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = fallbackDamage;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) result = CalculateTakingDamageModifiers(context.target, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}
