using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Gain Strength")]
public class GainStrengthAction : EnemyAction {
	public int amount;

	// 자기 강화는 버프 의도 아이콘 재사용
	public override string IntentIconName => "Buff";
	public override string IntentDescriptionTitle => "자기 버프";
	public override string IntentDescriptionKey => "EnemySelfBuffIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context) {
		return amount;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		return amount.ToString();
	}

	// 자기 자신에게 힘 부여
	public override void Execute(EnemyActionContext context) {
		Strength strength = new Strength();
		strength.Init(context.user, amount, 0);
		context.user.AddStatus(strength);

		context.user.PlaySkillAnimation();
	}
}
