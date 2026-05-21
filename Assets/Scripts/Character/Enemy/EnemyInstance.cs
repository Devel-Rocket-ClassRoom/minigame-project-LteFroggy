using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInstance : CharacterBase {
	[Header("=== 타겟팅 박스 Border들 ===")]
	[SerializeField] private Image[] _targetBoxBorders;

	[Header("=== 의도 렌더링 Prefab ===")]
	[SerializeField] private EnemyIntentRenderer _intentRendererPrefab;

	[Header("=== 자기 자신 렌더링할 이미지 ===")]
	[SerializeField] private SpriteRenderer _enemyRenderer;

	[Header("=== 의도 부모 아이콘 ===")]
	[SerializeField] private Transform _intentParent;
	
	[SerializeField] private EnemyData _enemyData;
	private EnemyManager _enemyManager;
	private int _turnCount;
	
	private readonly List<EnemyIntentRenderer> _intentRenderers = new();
	
	public void Init(EnemyData data, EnemyManager enemyManager) {
		// _enemyData = data;
		_enemyManager = enemyManager;
		_enemyRenderer.sprite = _enemyData.sprite;
		
		_turnCount = 0;
		
		// 시작 시에는 타겟박스 해제 상태로 시작
		SetTargetHighlight(false);
		
		// 시작 시에 의도 표시 표현
		MakeIntentIcon(_enemyManager.GetEnemyActionContext(this));
	}
	
	public override void OnTurnEnd() {
		base.OnTurnEnd();
		_turnCount++;
		
		// 턴 종료되면, 다음 턴에 내가 할 행동을 머리 위에 띄운다
		MakeIntentIcon(_enemyManager.GetEnemyActionContext(this));
	}
	
	public void MakeIntentIcon(BattleContext context) {
		// 허수아비 방어용 코드
		if (_enemyData == null) return;
		
		Debug.Log($"MakeIntentIcon 실행");
		
		// 이번 턴에 할 액션 리스트 받아오기
		EnemyActionPattern pattern = _enemyData.actions[_turnCount % _enemyData.actions.Count];

		Debug.Log($"이번 턴 액션 {pattern.actions.Count}개");
		
		// 이전 액션 아이콘 싹 비운다.
		foreach (var intent in _intentRenderers) { Destroy(intent.gameObject); }
		_intentRenderers.Clear();
		
		// 이번 턴 의도 아이콘 리스트로 다시 만들기
		foreach (var action in pattern.actions) {
			var newRenderer = Instantiate(_intentRendererPrefab, _intentParent);
			newRenderer.Init(action, context);
			
			_intentRenderers.Add(newRenderer);
		}
	} 
	
	public void UpdateIntentIcon(BattleContext context) {
		// 허수아비 방어용 코드
		if (_enemyData == null) return;
		
		// 지금 액션 아이콘 싹 다 다시 갱신시킨다.
		foreach (var renderer in _intentRenderers) {
			renderer.UpdateIntentInfo(context);
		}
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