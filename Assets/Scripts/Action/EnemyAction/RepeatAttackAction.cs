using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Repeat Deal Damage")]
public class RepeatAttackAction : EnemyAction {
	public int amount;
	public int repeat;

	public override string IntentIconName => "Attack";
	public override string IntentDescriptionTitle => "ê³µê²©";
	public override string IntentDescriptionKey => "EnemyAttackIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context, CalculationMode mode) {
		int result = amount;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) result = CalculateTakingDamageModifiers(context.target, result, mode);
		return result;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		var calculatedAmount = CalculatePreviewAmountWithContext(context);
		string amountText = calculatedAmount.ToString();
		if (calculatedAmount > amount) amountText = GetGreenText(amountText);
		if (calculatedAmount < amount) amountText = GetRedText(amountText);
		return $"{amountText}x{repeat}";
	}

	public override string GetIntentDescriptionWithContext(EnemyActionContext context) {
		return $"{CalculatePreviewAmountWithContext(context)} ?¼í•´ë¥?{repeat}ë²?ì¤ë‹ˆ??";
	}

	public override void Execute(EnemyActionContext context) {
		if (context.target.IsDead) return;
		var damageContext = new DamageContext(
			context.battleManager,
			context.user,
			context.target,
			null,
			null,
			DamageSourceType.Enemy
		);

		for (int i = 0; i < repeat; i++) {
			if (context.target.IsDead) break;
			context.target.GetDamage(ApplyAmountWithContext(context), damageContext);
		}

		context.user.PlayAttackAnimation();
	}
}
