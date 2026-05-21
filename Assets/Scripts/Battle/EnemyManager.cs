using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : BattleSystemManager {
	public List<EnemyInstance> EnemyList => _enemyList;
	[SerializeField] private List<EnemyInstance> _enemyList;

	[Header("=== BattleContext 생성을 위해 저장 ===")]
	[SerializeField] private CharacterManager _characterManager;
	[SerializeField] private BattleManager _battleManager;
	
	[HideInInspector] public UnityEvent OnEnemyTurnEnd;

	// 전투 시작 시에, 불러와야 할 적 리스트대로 생성
	public override void StartBattle() {
		// 만들었다 치자
		foreach (var enemy in _enemyList) {
			enemy.Init(null, this);
		}
	}

	public override void EndPlayerTurn() {
		StartCoroutine(CoEnemyTurn());
	}

	// 플레이어 턴 시작하면, 적 의도 Icon 모두 갱신

	private IEnumerator CoEnemyTurn() {
		foreach (EnemyInstance enemy in EnemyList) { enemy.OnTurnStart(); }
		
		// 이후 내부에서 적의 행동 모두 수행하는 코드 넣어주면 됨. 현재는 2초 대기로 일괄적으로 적용
		yield return new WaitForSeconds(2f);
		
		foreach (EnemyInstance enemy in EnemyList) { enemy.OnTurnEnd(); }
		OnEnemyTurnEnd?.Invoke();
	}
	
	public BattleContext GetEnemyActionContext(EnemyInstance instance) {
		// 적의 경우 공격받는 대상이 항상 Player
		// 공격하는 대상은 매개변수로 받아서 입력
		return new BattleContext(
			_battleManager,
			instance,
			null,
			_characterManager.Player
		);
	}
}