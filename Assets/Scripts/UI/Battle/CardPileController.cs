using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPileController : MonoBehaviour {
	[Header("=== 카드가 몇 장 있는지 표시할 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _countText;
	private Button _button;
	private DescriptionTooltipTrigger _tooltipTrigger;
	public RectTransform RectTransform => transform as RectTransform;

	private void Awake() {
		_button = GetComponent<Button>();
	}

	public void SetCountText(string text) => _countText.text = text;
	public void OnButtonPressed(UnityAction action) => _button.onClick.AddListener(action);

	public void SetTooltip(string title, string description) {
		if (_tooltipTrigger == null) {
			_tooltipTrigger = GetComponent<DescriptionTooltipTrigger>();
			if (_tooltipTrigger == null) _tooltipTrigger = gameObject.AddComponent<DescriptionTooltipTrigger>();
		}

		_tooltipTrigger.SetContent(title, description);
	}

	private void OnDisable() {
		_button.onClick.RemoveAllListeners();
	}
}
