using System.Collections;
using UnityEngine;

public class RepeatDealDamageAction : RepeatCardAction {
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
			context.target.GetDamage(CalculateAmountWithContext(context));
			context.user.PlayAttackAnimation();
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	protected override int CalculateAmountWithContext(CardUseContext context) {
		// 공격자 스탯 적용
		amount = context.user.CalculateAttackingDamage(amount);
		// 방어자 스탯 적용
		amount = context.target.CalculateGainingDamage(amount);
		// 유물 적용
		amount = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, amount);
		
		return amount;
	}
	
	protected override int CalculateRepeatWithContext(CardUseContext context) {
		// 유물 적용
		return context.relicManager.CalculateRepeatWithRelics(context.cardInfo, this, repeat);
	}
}