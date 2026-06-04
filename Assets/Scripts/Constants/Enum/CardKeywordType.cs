using System;

[Flags]
public enum CardKeywordType {
	None     = 0,
	Exhaust  = 1 << 0,  // 소모: 사용 후 이번 전투 동안 덱에서 제거
	Retain   = 1 << 1,  // 유지: 턴 종료 시 버리지 않고 손패에 남음
	Overload = 1 << 2,  // 과부하: 사용 시 다음 턴 에너지 -1
	Return   = 1 << 3,  // 귀환: 사용 후 다음 턴 손패로 돌아옴
	Innate   = 1 << 4,  // 선천: 매 전투 시작 시 무조건 손패에 포함
	Chain    = 1 << 5,  // 연쇄: 사용 시 덱 맨 위 카드 즉시 드로우
}
