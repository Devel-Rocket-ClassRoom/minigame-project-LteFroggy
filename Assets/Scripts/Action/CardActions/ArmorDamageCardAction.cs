using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Armor Damage")]
public class ArmorDamageCardAction : CardAction {
	protected override int Amount => 0;
	public override string CardDescriptionKey => "ArmorDamageCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override string GetCardDescriptionWithContext(CardUseContext context) =>
		StringTableManager.StringTable[CardDescriptionKey];

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		context.target.GetDamage(CalculateAmountWithContext(context));
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = context.user.Block;
		result = context.user.CalculateAttackingDamage(result);
		result = context.target.CalculateGainingDamage(result);
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}
}
