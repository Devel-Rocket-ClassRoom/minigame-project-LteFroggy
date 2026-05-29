using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Armor Damage")]
public class ArmorDamageCardAction : CardAction {
	protected override int Amount => 0;
	public override string CardDescriptionKey => "ArmorDamageCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey].Replace("@", "-");

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		context.target.GetDamage(CalculateAmountWithContext(context));
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = context.user.Block;
		result = context.user.CalculateAttackingDamage(result);
		if (context.target != null) { result = context.target.CalculateGainingDamage(result); }
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}
}
