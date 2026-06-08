public class Halve : StatusBase {
	public override string IconName => "HalveIcon";
	public override string TextToShow => Stack.ToString();
	public override bool IsActive => Stack > 0;

	public override void Merge(StatusBase status) { Stack += status.Stack; }
	public override void OnTurnEnd() { }

	public override int PreviewAttackingDamageModifier(int damage) {
		if (Stack <= 0) return damage;
		return damage / 2;
	}

	public override int ApplyAttackingDamageModifier(int damage) {
		int result = PreviewAttackingDamageModifier(damage);
		if (Stack > 0) Stack--;
		return result;
	}
}
