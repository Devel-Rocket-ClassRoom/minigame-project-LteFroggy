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
		return GetCardDescription();
	}

	public override void Execute(CardUseContext context) {
		if (context.target == null || context.target.IsDead) return;

		int lostHealth = context.user.MaxHealth - context.user.CurrentHealth;
		int damage = baseDamage + Mathf.RoundToInt(lostHealth * (lostHealthPercent / 100f));
		damage = context.user.CalculateAttackingDamage(damage);
		damage = context.target.CalculateGainingDamage(damage);
		damage = context.relicManager.CalculateAmountWithRelics(context, this, damage);
		context.DealDamage(context.target, this, damage);
		context.user.PlayAttackAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return baseDamage;
	}
}
