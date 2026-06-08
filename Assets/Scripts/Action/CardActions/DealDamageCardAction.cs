using System.ComponentModel.Design.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Deal Damage")]
public class DealDamageCardAction : CardAction {
	public int amount;
	public override bool IsDamageAction => true;
	
	// 특정 적 하나에게 데미지를 준다
	protected override int Amount => amount;
	public override string CardDescriptionKey => "AttackCardText";

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		context.DealDamage(context.target, this, ApplyAmountWithContext(context));
		
		// 공격 애니메이션 재생
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		// 사용자 기반 주는 데미지 계산
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		// 타겟이 있다면, 주는 데미지도 계산
		if (context.target != null) { result = CalculateTakingDamageModifiers(context.target, result, mode); }
		// 유물 기반으로 수정되는 데미지 있는지 계산
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		
		return result;
	}
}
