using TMPro;
using UnityEngine;

public class CardPile : MonoBehaviour {
	[Header("=== 카드가 몇 장 있는지 표시할 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _countText;
	
	public void SetCountText(string text) => _countText.text = text;
}