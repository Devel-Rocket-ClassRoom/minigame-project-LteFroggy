using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : BattleSystemManager {
	public List<EnemyInstance> EnemyList => _enemyList;
	private readonly List<EnemyInstance> _enemyList = new();

	[Header("=== BattleContext 생성을 위해 저장 ===")]
	[SerializeField] private CharacterManager _characterManager;
	[SerializeField] private BattleManager _battleManager;

	[Header("=== 적 생성을 위한 Prefab 저장 ===")]
	[SerializeField] private EnemyInstance _enemyPrefab;
	
	[HideInInspector] public UnityEvent OnEnemyTurnEnd;
	
	[Header("=== 임시로 스폰 가능한 적 데이터 넣어두기 ===")]
	[SerializeField] private EnemySpawnTable[] _tables;
	
	private readonly Vector3 _enemySpawnPoint = new(5.4f, 0f, 0f);
	private readonly float _enemySpawnSpacing = -3.3f;

	[HideInInspector] public UnityEvent OnEnemyAllDead; 

	// 전투 시작 시에, 불러와야 할 적 리스트대로 생성
	public override void StartBattle() {
		// 테이블에서 랜덤한 인스턴스 고르기
		var enemySpawnTable = _tables[Random.Range(0, _tables.Length)];
		
		for (int i = 0; i < enemySpawnTable.enemyList.Length; i++) {
			EnemyInstance enemy = Instantiate(_enemyPrefab);
			enemy.transform.position = _enemySpawnPoint + (Vector3.right * i * _enemySpawnSpacing);
			enemy.Init(enemySpawnTable.enemyList[i], this);
			
			_enemyList.Add(enemy);
		}
	}
	
	public void DeleteEnemy(EnemyInstance instance) {
		_enemyList.Remove(instance);
		
		// 적 모두 죽어서 사라졌으면 승리 처리
		if (_enemyList.Count == 0) {
			OnEnemyAllDead?.Invoke();
		}
	}

	public override void EndPlayerTurn() {
		StartCoroutine(CoEnemyTurn());
	}

	// 플레이어 턴 시작하면, 적 의도 Icon 모두 갱신
	private IEnumerator CoEnemyTurn() {
		foreach (EnemyInstance enemy in EnemyList) { enemy.OnTurnStart(); }
		
		// 적 각각이 자신의 행동 수행
		foreach (EnemyInstance enemy in EnemyList) {
			yield return enemy.CoExecutePattern();
		}
		
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