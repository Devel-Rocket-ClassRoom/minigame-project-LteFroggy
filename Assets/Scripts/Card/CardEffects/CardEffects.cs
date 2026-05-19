using UnityEngine;

public abstract class CardEffect : ScriptableObject {
	public abstract void Execute(BattleContext context);
	public virtual string GetPreviewText() => "";
	public virtual string GetPreviewTextWithContext(BattleContext ctx) => "";
}