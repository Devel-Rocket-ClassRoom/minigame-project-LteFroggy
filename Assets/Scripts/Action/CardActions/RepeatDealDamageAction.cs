using System.Collections;
using UnityEngine;

public class RepeatDealDamageAction : RepeatCardAction {
	public override bool IsDamageAction => true;
	public override string CardDescriptionKey => "RepeatAttackCardText";
	
	public int amount;
	public int repeat;
	protected override int Amount => amount;
	public override int Repeat => repeat;
	
	public override void Execute(CardUseContext context) {
		context.battleManager.StartCoroutine(ExecuteRepeat(context));
	}
	
	protected override IEnumerator ExecuteRepeat(CardUseContext context) {
		for (int i = 0; i < CalculateRepeatWithContext(context); i++) {
			// 데미지를 반복 횟수만큼 준다
			context.DealDamage(context.target, this, ApplyAmountWithContext(context));
			context.user.PlayAttackAnimation();
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) result = CalculateTakingDamageModifiers(context.target, result, mode);
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		return result;
	}
	
	protected override int CalculateRepeatWithContext(CardUseContext context) {
		// 유물 적용
		return context.relicManager.CalculateRepeatWithRelics(context, this, repeat);
	}
}
