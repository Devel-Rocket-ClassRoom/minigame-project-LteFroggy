
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Get Armor")]
public class GetArmorAction : CardAction {
	public int amount;
	
	public override void Execute(BattleContext context) {
		context.user.AddBlock(amount);
	}

	public override string GetCardDescription() {
		return StringTableManager.StringTable["DefenceCardText"].Replace("@", amount.ToString());
	}
}