using UnityEngine;

public abstract class StatusBase {
	public CharacterBase Owner { get; private set; }
	public int Stack { get; protected set;  }
	public int Duration { get; protected set; }
	
	public abstract string IconPath { get; }
	public Sprite Icon => Resources.Load<Sprite>(IconPath);
	
	public void Init(CharacterBase owner, int stack, int duration) {
		Owner = owner;
		Stack = stack;
		Duration = duration;
	}
	
	// 적용될 때 생길 일
	public virtual void OnApply() { }
	// 사라질 때 생길 일
	public virtual void OnRemove() { }
	// 턴 시작 시 생길 일
	public virtual void OnTurnStart() { }
	// 턴 종료 시 생길 일
	public virtual void OnTurnEnd() {
		Duration--;
	}
	
	public virtual int ModifyAttackingDamage(int damage) {
		return damage;
	}
	
	public virtual int ModifyDefendingDamage(int damage) {
		return damage;
	}
	
	public virtual int ModifyGainingArmor(int armor) {
		return armor;
	}
}