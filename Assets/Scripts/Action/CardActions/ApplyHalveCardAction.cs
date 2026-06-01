using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Apply Weak Point")]
public class ApplyWeakPointCardAction : CardAction {
	protected override int Amount => 0;
	public override string CardDescriptionKey => "WeakPointCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override string GetCardDescriptionWithContext(CardUseContext context) =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override void Execute(CardUseContext context) {
		if (context.target == null) return;
		var halve = new Halve();
		halve.Init(context.target, 1, 0);
		context.target.AddStatus(halve);
	}

	protected override int CalculateAmountWithContext(CardUseContext context) => 0;
}
