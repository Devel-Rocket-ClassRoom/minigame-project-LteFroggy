using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
	public int id;
	public int health;
	public List<List<EnemyBehavior>> actions = new();
}