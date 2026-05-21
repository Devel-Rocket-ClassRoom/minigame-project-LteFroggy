using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Draw Card")]
public class DrawCardAction : CardAction {
	public int amount;
	
	// 카드를 뽑는다.
	public override int Amount => amount;
	public override string CardDescriptionKey => "DrawCardText";

	public override void Execute(BattleContext context) {
		for (int i = 0; i < amount; i++) {
			context.manager.DeckManager.DrawCard();	
		}
	}
	
	public override int CalculateAmountWithContext(BattleContext context) {
		return amount;
	}
}