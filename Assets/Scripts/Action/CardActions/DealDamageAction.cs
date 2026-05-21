

using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Deal Damage")]
public class DealDamageAction : CardAction {
	public int amount;
	
	// 특정 적 하나에게 데미지를 준다
	public override int Amount => amount;
	public override string CardDescriptionKey => "AttackCardText";

	public override void Execute(BattleContext context) {
		context.target.GetDamage(CalculateAmountWithContext(context));
	}

	public override int CalculateAmountWithContext(BattleContext context) {
		int result = amount;
		// 사용자 기반 주는 데미지 계산
		result = context.user.CalculateAttackingDamage(result);
		// 타겟이 있다면, 주는 데미지도 계산
		if (context.target != null) { result = context.target.CalculateGainingDamage(result); }
		return result;
	}
}