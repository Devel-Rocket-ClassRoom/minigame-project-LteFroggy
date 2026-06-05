using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameMap/MapNodeConfig")]
public class MapNodeConfig : ScriptableObject {
	public MapNodeType Type;
	public Sprite Icon;
	public int Weight;
	public EnemySpawnTable[] EnemySpawnTables;
	public string SceneName => Type switch {
		MapNodeType.Battle => "BattleScene",
		MapNodeType.Boss   => "BattleScene",
		MapNodeType.Start  => "StartScene",
		MapNodeType.Rest   => "RestScene",
		MapNodeType.Event  => "EventScene",
		MapNodeType.Treasure => "EventScene",
		_ => throw new NotImplementedException()
	};
}
