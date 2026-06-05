public class EnemyActionContext : BattleContextBase {
	public readonly BattleManager battleManager;

	public EnemyActionContext(BattleManager battleManager, CharacterBase user, CharacterBase target) : base(user, target) {
		this.battleManager = battleManager;
	}
}
