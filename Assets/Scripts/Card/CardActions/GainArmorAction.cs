
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Get Armor")]
public class GainArmorAction : CardAction {
	public int amount;

	public override int Amount => amount;
	public override string DescriptionKey => "GainArmorCardText";

	public override void Execute(BattleContext context) {
		context.user.AddBlock(CalculateAmountWithContext(context));
	}

	public override int CalculateAmountWithContext(BattleContext ctx) {
		int result = amount;
		
		// 사용자의 Gain Block 양 기반으로 아머 얻기
		result = ctx.user.CalculateGainingArmor(result);
		return result;
	}
}