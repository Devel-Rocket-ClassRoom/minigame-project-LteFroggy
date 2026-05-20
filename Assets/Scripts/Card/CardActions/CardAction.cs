using UnityEngine;

public abstract class CardAction : ScriptableObject {
	public abstract void Execute(BattleContext context);
	public virtual string GetCardDescription() => "";
	public virtual string GetCardDescriptionWithContext(BattleContext ctx) => "";
	public virtual int CalculateAmountWithContext(BattleContext ctx) { return 0; }
}