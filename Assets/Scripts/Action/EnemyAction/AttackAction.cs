using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Actions/Deal Damage")]
public class AttackAction : EnemyAction {
	public int amount;

	public override string IntentIconName => "Attack";
	public override string IntentDescriptionTitle => "공격";
	public override string IntentDescriptionKey => "EnemyAttackIntentText";
	
	// 공격량은 실제 Amount와 동일하게 작성
	public override string GetIntentTextWithContext(EnemyActionContext context) {
		var calculatedAmount = CalculatePreviewAmountWithContext(context);
		// 피해량보다 높으면 초록색
		if (calculatedAmount > amount) { return GetGreenText(calculatedAmount.ToString()); }
		// 낮으면 빨간색
		if (calculatedAmount < amount) { return GetRedText(calculatedAmount.ToString()); }
		// 같으면 그냥
		return calculatedAmount.ToString();
	}

	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(EnemyActionContext context, CalculationMode mode) {
		int result = amount;
		// 사용자 기반 주는 데미지 계산
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		// 타겟이 있다면, 주는 데미지도 계산
		if (context.target != null) { result = CalculateTakingDamageModifiers(context.target, result, mode); }
		
		return result;
	}
	
	// 데미지 주기
	public override void Execute(EnemyActionContext context) {
		if (context.target.IsDead) return;
		var damageContext = new DamageContext(
			context.battleManager,
			context.user,
			context.target,
			null,
			null,
			DamageSourceType.Enemy
		);
		context.target.GetDamage(ApplyAmountWithContext(context), damageContext);
		context.user.PlayAttackAnimation();
	}
}
