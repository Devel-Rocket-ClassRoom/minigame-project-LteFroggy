using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Block Additional Draw")]
public class BlockAdditionalDrawCardAction : CardAction {
	protected override int Amount => 0;
	public override string CardDescriptionKey => "BlockAdditionalDrawCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override string GetCardDescriptionWithContext(CardUseContext context) =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override void Execute(CardUseContext context) {
		context.battleManager.DeckManager.BlockAdditionalDrawThisTurn = true;
	}

	protected override int CalculateAmountWithContext(CardUseContext context) => 0;
}
