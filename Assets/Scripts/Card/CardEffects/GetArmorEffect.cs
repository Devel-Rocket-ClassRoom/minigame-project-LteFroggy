
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Effects/Get Armor")]
public class GetArmorEffect : CardEffect {
	public int amount;
	
	public override void Execute(BattleContext context) {
		context.player.GetArmor(amount);
	}

	public override string GetPreviewText(BattleContext ctx) {
		return StringTableManager.StringTable["DefenceCardText"].Replace("@", amount.ToString());
	}
}