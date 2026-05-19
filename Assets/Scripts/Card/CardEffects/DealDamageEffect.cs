

using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Effects/Deal Damage")]
public class DealDamageEffect : CardEffect {
	public int amount;
	
	public override void Execute(BattleContext context) {
		context.target.GetDamage(amount);
	}

	public override string GetPreviewTextWithContext(BattleContext ctx) {
		return StringTableManager.StringTable["AttackCardText"].Replace("@", amount.ToString());
	}
}