using UnityEngine;

[CreateAssetMenu(menuName = "GameMap/MapNodeConfig")]
public class MapNodeConfig : ScriptableObject {
	public MapNodeType Type;
	public string DisplayName;
	public Sprite Icon;
	public int Weight;
}
