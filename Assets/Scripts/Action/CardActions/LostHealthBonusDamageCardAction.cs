using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Lost Health Bonus Damage")]
public class LostHealthBonusDamageCardAction : CardAction {
	public int baseDamage;
	public int lostHealthPercent = 20;
	public override bool IsDamageAction => true;

	protected override int Amount => baseDamage;
	public override string CardDescriptionKey => "LostHealthBonusDamageCardText";

	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", baseDamage.ToString())
			.Replace("#", lostHealthPercent.ToString());
	}

	public override string GetCardDescriptionWithContext(CardUseContext context) {
		string damageText = FormatPreviewAmount(CalculatePreviewAmountWithContext(context), baseDamage);
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("-", damageText)
			.Replace("#", lostHealthPercent.ToString());
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		context.DealDamage(context.target, this, ApplyAmountWithContext(context));
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int lostHealth = context.user.MaxHealth - context.user.CurrentHealth;
		int result = baseDamage + Mathf.RoundToInt(lostHealth * (lostHealthPercent / 100f));
		result = CalculateAttackingDamageModifiers(context.user, result, mode);
		if (context.target != null) result = CalculateTakingDamageModifiers(context.target, result, mode);
		return context.relicManager.CalculateAmountWithRelics(context, this, result);
	}
}
