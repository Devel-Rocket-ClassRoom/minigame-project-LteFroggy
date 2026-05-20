using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Draw Card")]
public class DrawCardAction : CardAction {
	public int amount;
	
	// 카드를 뽑는다.
	public override void Execute(BattleContext context) {
		
	}

	public override string GetCardDescription() {
		return StringTableManager.StringTable["DrawCardText"].Replace("@", amount.ToString());
	}
}