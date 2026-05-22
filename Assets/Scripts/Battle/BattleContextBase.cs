using System.Collections.Generic;

public abstract class BattleContextBase {
	public readonly CharacterBase user;
	public readonly CharacterBase target;
	
	public BattleContextBase(CharacterBase user, CharacterBase target) {
		this.user = user;
		this.target = target;
	}
}