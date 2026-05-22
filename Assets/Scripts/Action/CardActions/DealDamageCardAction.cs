using System.ComponentModel.Design.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction {
	public int amount;
	
	// 특정 적 하나에게 데미지를 준다
	protected override int Amount => amount;
	public override string CardDescriptionKey => "AttackCardText";

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		context.target.GetDamage(CalculateAmountWithContext(context));
		
		// 공격 애니메이션 재생
		context.user.PlayAttackAnimation();
		// 상대 피격 애니메이션 재생
		context.target.PlayHitAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		// 사용자 기반 주는 데미지 계산
		result = context.user.CalculateAttackingDamage(result);
		// 타겟이 있다면, 주는 데미지도 계산
		if (context.target != null) { result = context.target.CalculateGainingDamage(result); }
		// 유물 기반으로 수정되는 데미지 있는지 계산
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		
		return result;
	}
}