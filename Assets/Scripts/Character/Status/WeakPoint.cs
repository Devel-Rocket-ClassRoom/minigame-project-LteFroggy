public class WeakPoint : StatusBase {
	public override string IconName => "WeakPointIcon";
	public override string TextToShow => Stack.ToString();
	public override bool IsActive => Stack > 0;

	public override void Merge(StatusBase status) { Stack += status.Stack; }
	public override void OnTurnEnd() { }

	public override int ModifyAttackingDamage(int damage) {
		if (Stack <= 0) return damage;
		Stack--;
		return damage / 2;
	}
}
