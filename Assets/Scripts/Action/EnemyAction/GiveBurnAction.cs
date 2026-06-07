using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Give Burn")]
public class GiveBurnAction : EnemyAction {
	public int amount;

	public override string IntentIconName => "Burn";
	public override string IntentDescriptionTitle => "화상 부여";
	public override string IntentDescriptionKey => "EnemyBurnDebuffIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context) {
		return amount;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		return "";
	}

	public override void Execute(EnemyActionContext context) {
		if (context.target.IsDead) return;

		var burn = new Burn();
		burn.Init(context.target, CalculateAmountWithContext(context), 0);
		context.target.AddStatus(burn);

		context.user.PlayAttackAnimation();
	}
}
