public class BattleContext {
	public readonly CharacterBase user;
	public readonly CharacterBase[] targets;
	public readonly CharacterBase target;
	
	public BattleContext(CharacterBase user, CharacterBase[] targets, CharacterBase target) {
		this.user = user;
		this.targets = targets;
		this.target = target;
	}
}