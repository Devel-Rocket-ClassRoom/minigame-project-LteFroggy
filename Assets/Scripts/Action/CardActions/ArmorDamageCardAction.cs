using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Armor Damage")]
public class ArmorDamageCardAction : CardAction {
	public override bool IsDamageAction => true;
	protected override int Amount => 0;
	public override string CardDescriptionKey => "ArmorDamageCardText";

	public override string GetCardDescription() =>
		StringTableManager.StringTable[CardDescriptionKey].Replace("-", "-");

	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		context.DealDamage(context.target, this, ApplyAmountWithContext(context));
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = context.user.Block;
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) { result = CalculateTakingDamageModifiers(context.target, result, mode); }
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		return result;
	}
}
