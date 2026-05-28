using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPileController : MonoBehaviour {
	[Header("=== 카드가 몇 장 있는지 표시할 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _countText;
	private Button _button;

	private void Awake() {
		_button = GetComponent<Button>();
	}

	public void SetCountText(string text) => _countText.text = text;
	public void OnButtonPressed(UnityAction action) => _button.onClick.AddListener(action);

	private void OnDisable() {
		_button.onClick.RemoveAllListeners();
	}
}