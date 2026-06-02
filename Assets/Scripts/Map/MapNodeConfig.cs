using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameMap/MapNodeConfig")]
public class MapNodeConfig : ScriptableObject {
	public MapNodeType Type;
	public Sprite Icon;
	public int Weight;
	public string SceneName => Type switch {
		MapNodeType.Battle => "BattleScene",
		MapNodeType.Boss   => "BattleScene",
		MapNodeType.Start  => "StartScene",
		MapNodeType.Rest   => "RestScene",
		MapNodeType.Event  => "EventScene",
		_ => throw new NotImplementedException()
	};
}
