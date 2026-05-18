using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Effects/Draw Card")]
public class DrawCardEffect : CardEffect {
	public int amount;
	
	public override void Execute(BattleContext context) {
		
	}

	public override string GetPreviewText(BattleContext ctx) {
		return StringTableManager.StringTable["DrawCardText"].Replace("@", amount.ToString());
	}
}