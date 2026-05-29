using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Repeat Damage Apply Burn")]
public class RepeatDamageApplyBurnAction : RepeatCardAction {
	public int amount;
	public int repeat;

	protected override int Amount => amount;
	public override int Repeat => repeat;
	public override string CardDescriptionKey => "RepeatDamageApplyBurnCardText";

	public override void Execute(CardUseContext context) {
		context.battleManager.StartCoroutine(ExecuteRepeat(context));
	}

	protected override IEnumerator ExecuteRepeat(CardUseContext context) {
		int totalRepeat = CalculateRepeatWithContext(context);
		for (int i = 0; i < totalRepeat; i++) {
			if (context.target.IsDead) yield break;
			context.target.GetDamage(CalculateAmountWithContext(context));
			var burn = new Burn();
			burn.Init(context.target, 1, 0);
			context.target.AddStatus(burn);
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
