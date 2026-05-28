public class Strength : StatusBase {
	public override string IconName => "StrengthIcon";
	public override string TextToShow => Stack.ToString(); 
	public override bool IsActive => Stack != 0;
	public override void Merge(StatusBase status) {
		Stack += status.Stack;
	}
	
	// 힘은 줄어들 필요 없음.
	public override void OnTurnEnd() { }

	public override int ModifyAttackingDamage(int damage) {
		return damage += Stack;
	}
}