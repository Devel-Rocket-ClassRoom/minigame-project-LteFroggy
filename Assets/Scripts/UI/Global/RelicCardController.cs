using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] private TextMeshProUGUI _nameText;
	[SerializeField] private TextMeshProUGUI _effectText;
	[SerializeField] private TextMeshProUGUI _costText;
	[SerializeField] private TextMeshProUGUI _rarityText;
	[SerializeField] private Image _icon;
	[SerializeField] private GameObject _iconPlaceholder;
	[SerializeField] private GameObject _selectHighlight;
	[SerializeField] private Toggle _toggle;

	private RelicBase _relic;
	private bool _isHovering;

	public void Init(RelicBase relic, Action<RelicBase, Toggle, bool> onValueChanged) {
		_relic = relic;
		CacheReferences();

		if (_toggle != null) {
			_toggle.onValueChanged.RemoveAllListeners();
			_toggle.SetIsOnWithoutNotify(false);
			_toggle.onValueChanged.AddListener(isOn => onValueChanged?.Invoke(_relic, _toggle, isOn));
		}

		if (_nameText != null)
			_nameText.text = relic.displayName;
		if (_effectText != null)
			_effectText.text = DescriptionSystem.ProcessText(relic.effectDescription);
		if (_costText != null)
			_costText.text = relic.cost.ToString();
		if (_rarityText != null)
			_rarityText.text = StringTableManager.StringTable[relic.rarity.ToString()];

		SetIcon(relic.icon);
		SetSelected(false);
	}

	public void SetSelected(bool selected) {
		CacheReferences();
		if (_selectHighlight != null)
			_selectHighlight.SetActive(selected);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (_relic == null) return;

		_isHovering = true;
		DescriptionSystem.ProcessRelicPanel(_relic, (RectTransform)transform);
		if (_effectText != null)
			_effectText.text = DescriptionSystem.ProcessText(_relic.effectDescription);
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (!_isHovering) return;

		_isHovering = false;
		DescriptionSystem.Hide();
	}

	private void OnDisable() {
		if (!_isHovering) return;

		_isHovering = false;
		DescriptionSystem.Hide();
	}

	private void CacheReferences() {
		if (_toggle == null)
			_toggle = GetComponent<Toggle>();
		if (_nameText == null)
			_nameText = FindText("RelicName");
		if (_effectText == null)
			_effectText = FindText("EffectText");
		if (_costText == null)
			_costText = FindText("CostBadge/CostValue");
		if (_rarityText == null)
			_rarityText = FindText("RarityBadge/RarityText");
		if (_icon == null)
			_icon = FindImage("IconBg");
		if (_iconPlaceholder == null) {
			Transform iconBg = transform.Find("IconBg");
			Transform placeholder = iconBg != null ? iconBg.Find("IconPlaceholder") : null;
			_iconPlaceholder = placeholder != null ? placeholder.gameObject : null;
		}
		if (_selectHighlight == null) {
			Transform highlight = transform.Find("SelectHighlight");
			_selectHighlight = highlight != null ? highlight.gameObject : null;
		}
	}

	private void SetIcon(Sprite icon) {
		if (_icon == null) return;

		_icon.sprite = icon;
		_icon.preserveAspect = true;
		if (_iconPlaceholder != null)
			_iconPlaceholder.SetActive(icon == null);
	}

	private TextMeshProUGUI FindText(string path) {
		Transform child = transform.Find(path);
		return child != null ? child.GetComponent<TextMeshProUGUI>() : null;
	}

	private Image FindImage(string path) {
		Transform child = transform.Find(path);
		return child != null ? child.GetComponent<Image>() : null;
	}
}
