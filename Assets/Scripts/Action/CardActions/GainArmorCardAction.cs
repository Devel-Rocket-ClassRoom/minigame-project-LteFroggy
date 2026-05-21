
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Gain Armor")]
public class GainArmorCardAction : CardAction {
	public int amount;

	public override int Amount => amount;
	public override string CardDescriptionKey => "GainArmorCardText";

	public override void Execute(BattleContext context) {
		context.user.AddBlock(CalculateAmountWithContext(context));
	}

	public override int CalculateAmountWithContext(BattleContext context) {
		int result = amount;
		
		// 사용자의 Gain Block 양 기반으로 아머 얻기
		result = context.user.CalculateGainingArmor(result);
		return result;
	}
}