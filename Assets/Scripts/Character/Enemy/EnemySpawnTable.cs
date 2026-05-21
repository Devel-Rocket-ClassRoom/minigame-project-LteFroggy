using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Spawn Table")]
public class EnemySpawnTable : ScriptableObject {
	public EnemyData[] enemyList;
}