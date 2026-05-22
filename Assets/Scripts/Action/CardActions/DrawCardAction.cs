using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Draw Card")]
public class DrawCardAction : CardAction {
	public int amount;
	
	// 카드를 뽑는다.
	protected override int Amount => amount;
	public override string CardDescriptionKey => "DrawCardText";

	public override void Execute(CardUseContext context) {
		context.user.PlaySkillAnimation();
		
		for (int i = 0; i < amount; i++) {
			context.battleManager.DeckManager.DrawCard();
		}
	}
	
	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		// 유물 기반으로 수정되는 값 있는지 계산
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		
		return amount;
	}
}