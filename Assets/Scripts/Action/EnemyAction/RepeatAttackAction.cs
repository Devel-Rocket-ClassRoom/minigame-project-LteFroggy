using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Repeat Deal Damage")]
public class RepeatAttackAction : EnemyAction {
	public int amount;
	public int repeat;

	public override string IntentIconName => "Attack";
	public override string IntentDescriptionTitle => "공격";
	public override string IntentDescriptionKey => "EnemyAttackIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context) {
		int result = amount;
		result = context.user.CalculateAttackingDamage(result);
		if (context.target != null) result = context.target.CalculateGainingDamage(result);
		return result;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		var calculatedAmount = CalculateAmountWithContext(context);
		string amountText = calculatedAmount.ToString();
		if (calculatedAmount > amount) amountText = GetGreenText(amountText);
		if (calculatedAmount < amount) amountText = GetRedText(amountText);
		return $"{amountText}x{repeat}";
	}

	public override string GetIntentDescriptionWithContext(EnemyActionContext context) {
		return $"{CalculateAmountWithContext(context)} 피해를 {repeat}번 줍니다.";
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
			context.target.GetDamage(CalculateAmountWithContext(context), damageContext);
		}

		context.user.PlayAttackAnimation();
	}
}
