public class Weakness : StatusBase {
	public override string IconName => "WeaknessIcon";
	public override string TextToShow => Duration.ToString();
	public override bool IsActive => Duration > 0;

	// 취약 걸리면 받는 데미지 50% 증가
	public override int ModifyGainingDamage(int damage) {
		return damage = (int)(damage * 1.5f);
	}

	// 취약은 합쳐질 때 턴만 더해진다.
	public override void Merge(StatusBase status) {
		Duration += status.Duration;
	}
}