using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	[Header("=== 세팅할 정보 UI ===")]
	[SerializeField] private Image _cardIcon;
	[SerializeField] private TextMeshProUGUI _cardNameText;
	[SerializeField] private TextMeshProUGUI _cardDescriptionText;
	[SerializeField] private TextMeshProUGUI _cardCostText;

	private Button _button;
	private Graphic _targetGraphic;
	private CardInstance _cardInstance;
	private bool _isHovering;
	private Color _defaultGraphicColor;
	private bool _hasDefaultGraphicColor;

	/// <summary>
	/// CardView 초기화할 때 사용
	/// </summary>
	/// <param name="instance">담길 카드의 정보</param>
	/// <param name="action">카드가 클릭될 때 수행될 액션</param>
	public void Init(CardInstance instance, UnityAction<CardInstance> action = null) {
		_cardInstance = instance;

		CacheButton();
		_button.onClick.RemoveAllListeners();
		_button.onClick.AddListener(() => {
			DescriptionSystem.Hide();
			_isHovering = false;
			action?.Invoke(_cardInstance);
		});
		SetSelected(false);
		SetInteractable(true);

		_cardIcon.sprite = _cardInstance.Icon;
		_cardNameText.text = _cardInstance.CardName;
		_cardNameText.fontSize = 40 - 2 * _cardInstance.CardName.Length;
		_cardDescriptionText.text = DescriptionSystem.ProcessText(_cardInstance.GetCardDescription());
		_cardCostText.text = _cardInstance.Cost.ToString();
	}

	public void SetSelected(bool selected) {
		CacheButton();
		if (_targetGraphic == null) return;

		_targetGraphic.color = selected
			? new Color(0.95f, 0.72f, 0.24f, 1f)
			: _defaultGraphicColor;
	}

	public void SetInteractable(bool interactable) {
		CacheButton();
		_button.interactable = interactable;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		_isHovering = true;
		_cardDescriptionText.text = DescriptionSystem.ProcessCardText(
			_cardInstance.GetCardDescription(),
			(RectTransform)transform
		);
	}

	public void OnPointerExit(PointerEventData eventData) {
		_isHovering = false;
		DescriptionSystem.Hide();
	}

	private void OnDisable() {
		// 호버 중에 카드가 비활성화되면 OnPointerExit가 호출되지 않으므로 패널을 직접 정리
		if (_isHovering) {
			DescriptionSystem.Hide();
			_isHovering = false;
		}
	}

	private void CacheButton() {
		if (_button == null) _button = GetComponent<Button>();
		if (_targetGraphic == null) _targetGraphic = _button != null ? _button.targetGraphic : GetComponent<Graphic>();
		if (_targetGraphic != null && !_hasDefaultGraphicColor) {
			_defaultGraphicColor = _targetGraphic.color;
			_hasDefaultGraphicColor = true;
		}
	}
}
