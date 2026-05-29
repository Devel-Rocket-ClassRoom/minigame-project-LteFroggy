using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Last Hit Double Repeat Damage")]
public class LastHitDoubleRepeatDamageAction : RepeatCardAction {
	public int amount;
	public int repeat;

	protected override int Amount => amount;
	public override int Repeat => repeat;
	public override string CardDescriptionKey => "LastHitDoubleRepeatDamageCardText";

	public override void Execute(CardUseContext context) {
		context.battleManager.StartCoroutine(ExecuteRepeat(context));
	}

	protected override IEnumerator ExecuteRepeat(CardUseContext context) {
		int totalRepeat = CalculateRepeatWithContext(context);
		for (int i = 0; i < totalRepeat; i++) {
			if (context.target.IsDead) yield break;
			int damage = CalculateAmountWithContext(context);
			if (i == totalRepeat - 1) damage *= 2;
			context.target.GetDamage(damage);
			context.user.PlayAttackAnimation();
			yield return new UnityEngine.WaitForSeconds(0.5f);
		}
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.user.CalculateAttackingDamage(result);
		result = context.target.CalculateGainingDamage(result);
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}

	protected override int CalculateRepeatWithContext(CardUseContext context) {
		return context.relicManager.CalculateRepeatWithRelics(context.cardInfo, this, repeat);
	}
}
