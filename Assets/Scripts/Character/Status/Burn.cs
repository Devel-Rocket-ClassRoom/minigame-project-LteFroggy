public class Burn : StatusBase {
	public override string IconName => "BurnIcon";
	
	public override string TextToShow => Stack.ToString();
	
	public override bool IsActive => Stack != 0;
	
	public override void Merge(StatusBase status) {
		Stack += status.Stack;
	}

	// 화상도 턴 감소 없음
	public override void OnTurnEnd() { }

	// 턴 시작할 때 데미지 주기
	public override void OnTurnStart() {
		Owner.GetDamage(Stack);
	}
}