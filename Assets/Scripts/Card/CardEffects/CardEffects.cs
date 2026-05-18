using UnityEngine;

public abstract class CardEffect : ScriptableObject {
	public abstract void Execute(BattleContext context);
	public virtual string GetPreviewText(BattleContext ctx) => "";
}