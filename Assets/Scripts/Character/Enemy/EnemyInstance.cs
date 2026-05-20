using UnityEngine;
using UnityEngine.UI;

public class EnemyInstance : CharacterBase {
	[Header("=== 타겟팅 박스 Border들 ===")]
	[SerializeField] private Image[] _targetBoxBorders;
	private EnemyData _enemyData;

	// 시작 시에는 타겟박스 해제 상태로 시작
	private void Start() {
		SetTargetHighlight(false);
	}
	
	public override void SetHealth() {
		// Data에서 가져오도록 해야 함
		// 일단 기본 세팅
		MaxHealth = 10;
		CurrentHealth = MaxHealth;
	}

	public void SetTargetHighlight(bool activate) {
		foreach (var border in _targetBoxBorders) {
			border.enabled = activate;
		}
	}
}