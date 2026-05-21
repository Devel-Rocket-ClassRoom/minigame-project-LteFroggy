

using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Deal Damage")]
public class DealDamageAction : CardAction {
	public int amount;
	
	// 특정 적 하나에게 데미지를 준다
	public override int Amount => amount;
	public override string DescriptionKey => "AttackCardText";

	public override void Execute(BattleContext context) {
		context.target.GetDamage(CalculateAmountWithContext(context));
	}

	public override int CalculateAmountWithContext(BattleContext ctx) {
		int result = amount;
		// 사용자 기반 주는 데미지 계산
		result = ctx.user.CalculateAttackingDamage(result);
		// 타겟이 있다면, 주는 데미지도 계산
		if (ctx.target != null) { result = ctx.target.CalculateDefendingDamage(result); }
		return result;
	}
}