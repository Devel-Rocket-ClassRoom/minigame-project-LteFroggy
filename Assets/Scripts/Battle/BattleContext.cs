using System.Collections.Generic;

public class BattleContext {
	public readonly CharacterBase user;
	public readonly List<CharacterBase> targets;
	public readonly CharacterBase target;
	public BattleManager manager;
	
	public BattleContext(BattleManager manager, PlayerCharacter user, List<CharacterBase> targets, EnemyInstance target) {
		this.user = user;
		this.targets = targets;
		this.target = target;
		this.manager = manager;
	}
}