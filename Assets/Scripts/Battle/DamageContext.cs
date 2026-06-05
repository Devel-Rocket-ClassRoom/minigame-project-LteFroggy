public class DamageContext {
	public readonly BattleManager battleManager;
	public readonly CharacterBase source;
	public readonly CharacterBase target;
	public readonly CardUseContext cardContext;
	public readonly CardAction action;
	public readonly DamageSourceType sourceType;
	public readonly bool ignoresBlock;
	public readonly bool isSelfDamage;

	public DamageContext(
		BattleManager battleManager,
		CharacterBase source,
		CharacterBase target,
		CardUseContext cardContext,
		CardAction action,
		DamageSourceType sourceType,
		bool ignoresBlock = false,
		bool isSelfDamage = false
	) {
		this.battleManager = battleManager;
		this.source = source;
		this.target = target;
		this.cardContext = cardContext;
		this.action = action;
		this.sourceType = sourceType;
		this.ignoresBlock = ignoresBlock;
		this.isSelfDamage = isSelfDamage;
	}
}
