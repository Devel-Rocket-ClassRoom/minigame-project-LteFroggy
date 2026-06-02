public class Weakness : StatusBase {
	public override string IconName => "WeaknessIcon";
	public override string TextToShow => Duration.ToString();
	public override bool IsActive => Duration > 0;

	// 취약 걸리면 주는 데미지 25% 감소로 수정..
	public override int ModifyAttackingDamage(int damage) {
		return damage = (int)(damage * 0.75f);
	}

	// 취약은 합쳐질 때 턴만 더해진다.
	public override void Merge(StatusBase status) {
		Duration += status.Duration;
	}
}