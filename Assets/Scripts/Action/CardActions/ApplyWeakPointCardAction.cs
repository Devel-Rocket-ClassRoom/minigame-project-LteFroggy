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
		var weakPoint = new WeakPoint();
		weakPoint.Init(context.target, 1, 0);
		context.target.AddStatus(weakPoint);
	}

	protected override int CalculateAmountWithContext(CardUseContext context) => 0;
}
