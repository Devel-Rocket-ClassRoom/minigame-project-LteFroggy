using System.Collections.Generic;

public class CardUseContext : BattleContextBase {
	public readonly List<CharacterBase> targets;
	public readonly BattleManager manager;
	public readonly CardInstance cardInfo;
	public CardUseContext(BattleManager manager, CharacterBase user, List<CharacterBase> targets, CharacterBase target, CardInstance cardInfo) : base(user, target) {
		this.targets = targets;
		this.manager = manager;
		this.cardInfo = cardInfo;
	}
}