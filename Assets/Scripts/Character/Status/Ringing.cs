using UnityEngine;

public class Ringing : StatusBase {
	// 이번 턴에 카드를 이미 사용했는지
	private bool _usedCardThisTurn;

	public override string IconName => "RingingIcon";
	public override string TextToShow => Duration.ToString();
	public override bool IsActive => Duration > 0;

	public override void Merge(StatusBase status) {
		Duration = Mathf.Max(Duration, status.Duration);
	}

	// 공명: 이번 턴에 카드를 1장만 사용할 수 있음
	public override bool CanUseCard() => !_usedCardThisTurn;
	public override string CardUseBlockedMessageKey => _usedCardThisTurn ? "StatusCardUseBlocked" : null;
	public override void OnCardUsed() => _usedCardThisTurn = true;

	// 턴 시작 시 사용 가능 상태로 초기화
	public override void OnTurnStart() => _usedCardThisTurn = false;
}
