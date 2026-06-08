using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Give Ringing")]
public class GiveRingingAction : EnemyAction {
	public int amount;

	public override string IntentIconName => "Debuff";
	public override string IntentDescriptionTitle => "대상 디버프";
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

		Ringing ringing = new Ringing();
		ringing.Init(context.target, 0, ApplyAmountWithContext(context));
		context.target.AddStatus(ringing);

		context.user.PlaySkillAnimation();
	}
}