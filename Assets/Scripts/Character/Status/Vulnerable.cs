public class Vulnerable : StatusBase {
	public override string IconName => "VulnerableIcon";
	public override string TextToShow => Duration.ToString();
	public override bool IsActive => Duration > 0;
	
	public override void Merge(StatusBase status) {
		Duration += status.Duration;
	}

	public override int ModifyGainingDamage(int damage) {
		return damage = (int)(damage * 1.5f);
	}
}