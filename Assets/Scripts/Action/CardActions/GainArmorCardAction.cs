
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Gain Armor")]
public class GainArmorCardAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "GainArmorCardText";

	public override void Execute(CardUseContext context) {
		context.user.AddBlock(CalculateAmountWithContext(context));
		
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		
		// 사용자의 Gain Block 양 기반으로 아머 얻기
		result = context.user.CalculateGainingArmor(result);
		// 유물 기반으로 수정되는 값 있는지 계산
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		
		return result;
	}
}