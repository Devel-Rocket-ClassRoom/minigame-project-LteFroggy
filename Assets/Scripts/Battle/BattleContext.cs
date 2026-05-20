using System.Collections.Generic;

public class BattleContext {
	public readonly PlayerCharacter user;
	public readonly List<EnemyInstance> targets;
	public readonly EnemyInstance target;
	public BattleManager manager;
	
	public BattleContext(BattleManager manager, PlayerCharacter user, List<EnemyInstance> targets, EnemyInstance target) {
		this.user = user;
		this.targets = targets;
		this.target = target;
		this.manager = manager;
	}
}