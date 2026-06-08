using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Gain Strength")]
public class GainStrengthAction : EnemyAction {
	public int amount;

	public override string IntentIconName => "Buff";
	public override string IntentDescriptionTitle => "자기 버프";
	public override string IntentDescriptionKey => "EnemySelfBuffIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context, CalculationMode mode) {
		return amount;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		return amount.ToString();
	}

	public override void Execute(EnemyActionContext context) {
		Strength strength = new Strength();
		strength.Init(context.user, amount, 0);
		context.user.AddStatus(strength);

		context.user.PlaySkillAnimation();
	}
}