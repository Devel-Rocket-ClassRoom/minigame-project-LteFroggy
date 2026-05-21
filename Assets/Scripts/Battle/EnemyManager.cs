using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : BattleSystemManager {
	public List<EnemyInstance> EnemyList => _enemyList;
	[SerializeField] private List<EnemyInstance> _enemyList;

	[HideInInspector] public UnityEvent OnEnemyTurnEnd;
	
	public override void EndPlayerTurn() {
		StartCoroutine(CoEnemyTurn());
	}
	
	private IEnumerator CoEnemyTurn() {
		foreach (EnemyInstance enemy in EnemyList) { enemy.OnTurnStart(); }
		
		// 이후 내부에서 적의 행동 모두 수행하는 코드 넣어주면 됨. 현재는 2초 대기로 일괄적으로 적용
		yield return new WaitForSeconds(2f);
		
		
		foreach (EnemyInstance enemy in EnemyList) { enemy.OnTurnEnd(); }
		OnEnemyTurnEnd?.Invoke();
	}
}