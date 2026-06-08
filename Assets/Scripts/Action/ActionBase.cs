using UnityEngine;

public enum CalculationMode {
	Preview,
	Apply
}

public abstract class ActionBase<TContext> : ScriptableObject where TContext : BattleContextBase {
	protected abstract int Amount { get; }

	protected abstract int CalculateAmountWithContext(TContext context, CalculationMode mode);

	protected int CalculatePreviewAmountWithContext(TContext context) {
		return CalculateAmountWithContext(context, CalculationMode.Preview);
	}

	protected int ApplyAmountWithContext(TContext context) {
		return CalculateAmountWithContext(context, CalculationMode.Apply);
	}

	protected int CalculateAttackingDamageModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyAttackingDamageModifiers(amount)
			: character.PreviewAttackingDamageModifiers(amount);
	}

	protected int CalculateTakingDamageModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyTakingDamageModifiers(amount)
			: character.PreviewTakingDamageModifiers(amount);
	}

	protected int CalculateGainingArmorModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyGainingArmorModifiers(amount)
			: character.PreviewGainingArmorModifiers(amount);
	}

	protected int CalculateGivingBurnModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyGivingBurnModifiers(amount)
			: character.PreviewGivingBurnModifiers(amount);
	}

	protected string GetGreenText(string text) => $"<color=#00FF00>{text}</color>";
	protected string GetRedText(string text) => $"<color=#FF0000>{text}</color>";

	public abstract void Execute(TContext context);
}