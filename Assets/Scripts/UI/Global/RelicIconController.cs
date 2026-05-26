using UnityEngine;
using UnityEngine.UI;

public class RelicIconController : MonoBehaviour {
	[Header("=== 유물 아이콘 ===")]
	[SerializeField] private Image _icon;

	public void Set(RelicBase relic) {
		_icon.sprite = relic.icon;
	}
}
