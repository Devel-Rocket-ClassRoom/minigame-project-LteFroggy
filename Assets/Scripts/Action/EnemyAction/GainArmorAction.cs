using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Gain Armor")]
public class GainArmorAction : EnemyAction {
	public int amount;
	protected override int Amount => amount;
	public override string IntentIconName => $"Defend";
	public override string IntentDescriptionTitle => "방어";
	public override string IntentDescriptionKey => "EnemyGainArmorIntentText";
	
	protected override int CalculateAmountWithContext(EnemyActionContext context, CalculationMode mode) {
		// 본인 강화값에 기반한 강화도 보기
		return CalculateGainingArmorModifiers(context.user, amount, mode);
	}
	
	public override void Execute(EnemyActionContext context) {
		context.user.AddBlock(ApplyAmountWithContext(context));
		context.user.PlaySkillAnimation();
	}
	
	// 얼마 얻을지 알려주지 않음
	public override string GetIntentTextWithContext(EnemyActionContext context) { return ""; }

}
