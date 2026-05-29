using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
	public int id;
	public int health;
	public string name;
	public Sprite sprite;
	public RuntimeAnimatorController animatorController;
	public List<EnemyActionPattern> actions = new();
}