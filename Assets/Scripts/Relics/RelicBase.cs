using UnityEngine;

public abstract class RelicBase {
	public abstract string relicId { get; }
	
	public Sprite icon => Resources.Load<Sprite>($"Sprites/Relics/{GetType().Name}");
	public abstract RelicRarity rarity { get; }
	
	public virtual void OnTurnStart() { }
	public virtual void OnTurnEnd() { }
	
	public virtual int CalculateAmount(CardAction action, CardInstance instance, int amount) { return amount; }
	public virtual int CalculateRepeat(CardAction action, CardInstance instance, int repeat) { return repeat; }
}