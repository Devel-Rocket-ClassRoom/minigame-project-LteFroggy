using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Repeat Damage Apply Burn")]
public class RepeatDamageApplyBurnAction : RepeatDealDamageAction {
	public override string CardDescriptionKey => "RepeatDamageApplyBurnCardText";

	protected override IEnumerator ExecuteRepeat(CardUseContext context) {
		int totalRepeat = CalculateRepeatWithContext(context);
		for (int i = 0; i < totalRepeat; i++) {
			if (context.target.IsDead) yield break;
			context.target.GetDamage(CalculateAmountWithContext(context));
			var burn = new Burn();
			burn.Init(context.target, 1, 0);
			context.target.AddStatus(burn);
			context.user.PlayAttackAnimation();
			yield return new WaitForSeconds(0.5f);
		}
	}
}
