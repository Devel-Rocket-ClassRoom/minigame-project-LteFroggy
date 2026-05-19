

using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Deal Damage")]
public class DealDamageAction : CardAction {
	public int amount;
	
	public override void Execute(BattleContext context) {
		context.target.GetDamage(amount);
	}

	public override string GetCardDescription() {
		return StringTableManager.StringTable["AttackCardText"].Replace("@", amount.ToString());
	}
}