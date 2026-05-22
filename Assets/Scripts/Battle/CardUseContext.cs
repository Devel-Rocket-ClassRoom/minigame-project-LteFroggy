using System.Collections.Generic;

public class CardUseContext : BattleContextBase {
	public readonly List<CharacterBase> targets;
	public readonly CardInstance cardInfo;
	public readonly BattleManager battleManager;
	public readonly RelicManager relicManager;
	
	public CardUseContext(BattleManager battleManager, RelicManager relicManager, CharacterBase user, List<CharacterBase> targets, CharacterBase target, CardInstance cardInfo) : base(user, target) {
		this.targets = targets;
		this.battleManager = battleManager;
		this.relicManager = relicManager;
		this.cardInfo = cardInfo;
	}
}