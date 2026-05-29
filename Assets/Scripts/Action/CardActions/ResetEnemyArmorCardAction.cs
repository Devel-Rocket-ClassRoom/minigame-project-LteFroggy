using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Reset Enemy Armor")]
public class ResetEnemyArmorCardAction : CardAction {
	protected override int Amount => 0;
	public override string CardDescriptionKey => "ResetEnemyArmorCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override string GetCardDescriptionWithContext(CardUseContext context) =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override void Execute(CardUseContext context) {
		if (context.target == null) return;
		context.target.ClearBlock();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) => 0;
}
