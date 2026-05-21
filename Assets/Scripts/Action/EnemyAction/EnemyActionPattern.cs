using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Pattern")]
public class EnemyActionPattern : ScriptableObject {
	public List<EnemyAction> actions = new();
}