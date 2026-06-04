using UnityEngine;

public abstract class StatusBase {
	public CharacterBase Owner { get; private set; }
	
	public int Stack { get; protected set;  }
	
	public int Duration { get; protected set; }
	
	public abstract string IconName { get; }
	
	public Sprite Icon => Resources.Load<Sprite>($"Sprites/Statuses/{IconName}");
	
	// 화면에 표시할 키워드 (StringTable 키와 일치)
	public virtual string DescriptionTitle => StringTableManager.StringTable[GetType().Name];
	public virtual string DescriptionContent => StringTableManager.DescriptionTable[DescriptionTitle];
	
	// 상태이상 창 아래에 보일 텍스트
	public abstract string TextToShow { get; }
	
	// 해당 상태이상이 아직 적용되는건지
	public abstract bool IsActive { get;}
	
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
	public virtual void OnTurnEnd() { Duration--; }
	
	public virtual int ModifyAttackingDamage(int damage) {
		return damage;
	}
	
	public virtual int ModifyGainingDamage(int damage) {
		return damage;
	}
	
	public virtual int ModifyGainingArmor(int armor) {
		return armor;
	}
	
	public virtual int ModifyGivingBurn(int burn) {
		return burn;
	}

	public virtual int ModifyStartingEnergy(int energy) {
		return energy;
	}

	// 이 상태이상이 카드 사용을 허용하는지 (기본값: 허용)
	public virtual bool CanUseCard() => true;
	// 카드를 사용했을 때 호출 (사용 횟수 추적 등)
	public virtual void OnCardUsed() { }

	// 같은 종류의 상태이상과 합쳐질 때 합쳐지는 방법
	public abstract void Merge(StatusBase status);
}