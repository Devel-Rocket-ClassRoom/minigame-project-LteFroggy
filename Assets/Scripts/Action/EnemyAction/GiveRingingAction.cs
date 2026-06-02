using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Give Ringing")]
public class GiveRingingAction : EnemyAction {
	public int amount;

	// 상대에게 거는 디버프는 약화 의도 아이콘 재사용
	public override string IntentIconName => "Debuff";
	public override string IntentDescriptionTitle => "상대 디버프";
	public override string IntentDescriptionKey => "EnemyTargetDebuffIntentText";

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context) {
		return amount;
	}

	public override string GetIntentTextWithContext(EnemyActionContext context) {
		return "";
	}

	// 상대(플레이어)에게 공명 부여
	public override void Execute(EnemyActionContext context) {
		if (context.target.IsDead) return;

		Ringing ringing = new Ringing();
		ringing.Init(context.target, 0, amount);
		context.target.AddStatus(ringing);

		context.user.PlaySkillAnimation();
	}
}
