using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInstance : MonoBehaviour {
	[Header("=== 타겟팅 박스 Border들 ===")]
	[SerializeField] private Image[] _targetBoxBorders;
	private EnemyData _enemyData;

	// 시작 시에는 타겟박스 해제 상태로 시작
	private void Start() {
		SetTargetHighlight(false);
	}

	public void SetTargetHighlight(bool activate) {
		foreach (var border in _targetBoxBorders) {
			border.enabled = activate;
		}
	}
}