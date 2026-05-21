using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Gain Armor")]
public class GainArmorAction : EnemyAction {
	public int amount;
	public override int Amount => amount;
	public override string IntentIconName => $"Defend";
	
	public override int CalculateAmountWithContext(BattleContext context) {
		// 본인 강화값에 기반한 강화도 보기
		return context.user.CalculateGainingArmor(amount);
	}
	
	public override void Execute(BattleContext context) {
		context.user.AddBlock(CalculateAmountWithContext(context));
	}
	
	// 얼마 얻을지 알려주지 않음
	public override string GetIntentTextWithContext(BattleContext context) { return ""; }
}