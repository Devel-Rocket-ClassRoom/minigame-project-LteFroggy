using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Last Hit Double Repeat Damage")]
public class LastHitDoubleRepeatDamageAction : RepeatDealDamageAction {
	public override string CardDescriptionKey => "LastHitDoubleRepeatDamageCardText";

	protected override IEnumerator ExecuteRepeat(CardUseContext context) {
		int totalRepeat = CalculateRepeatWithContext(context);
		for (int i = 0; i < totalRepeat; i++) {
			if (context.target.IsDead) yield break;
			int damage = CalculateAmountWithContext(context);
			if (i == totalRepeat - 1) damage *= 2;
			context.target.GetDamage(damage);
			context.user.PlayAttackAnimation();
			yield return new WaitForSeconds(0.5f);
		}
	}
}
