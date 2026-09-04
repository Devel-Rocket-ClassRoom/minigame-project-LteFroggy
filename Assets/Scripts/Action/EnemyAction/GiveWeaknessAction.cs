using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Give Weakness")]
public class GiveWeaknessAction : EnemyAction {
	public int amount;

	public override string IntentIconName => "Debuff";
	public override string IntentDescriptionTitle => "약화 효과";
	public override string IntentDescriptionKey => "EnemyTargetDebuffIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context, CalculationMode mode) {
		return amount;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		return "";
	}

	public override void Execute(EnemyActionContext context) {
		if (context.target.IsDead) return;

		var weakness = new Weakness();
		weakness.Init(context.target, 0, ApplyAmountWithContext(context));
		context.target.AddStatus(weakness);

		context.user.PlayAttackAnimation();
	}
}
