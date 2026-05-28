using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalHUDController : MonoBehaviour {
	[Header("=== 상태 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _hpText;
	[SerializeField] private TextMeshProUGUI _goldText;

	[Header("=== 버튼 ===")]
	[SerializeField] private Button _mapButton;
	[SerializeField] private Button _deckButton;

	[Header("=== 유물 목록 ===")]
	[SerializeField] private Transform _relicRow;
	[SerializeField] private RelicIconController _relicIconPrefab;
	[SerializeField] private TMP_FontAsset _relicFont;

	[Header("=== 오버레이 패널 ===")]
	[SerializeField] private OverlayPanelController _mapOverlay;

	private const float RelicRowHeight = 88f;
	private const float RelicLabelWidth = 50f;

	private GamePlayData _gamePlayData;

	private void OnEnable() {
		_gamePlayData = GamePlayData.Instance;
		_gamePlayData.OnHealthChanged += OnHealthChanged;
		_gamePlayData.OnGoldChanged += OnGoldChanged;
		_gamePlayData.OnRelicsChanged += OnRelicsChanged;

		GameEvents.OnNodeCompleted += _mapOverlay.Open;
		GameEvents.OnNextNodeSelected += _mapOverlay.Close;
	}

	private void OnDisable() {
		GameEvents.OnNodeCompleted -= _mapOverlay.Open;
		GameEvents.OnNextNodeSelected -= _mapOverlay.Close;

		if (_gamePlayData == null) return;
		_gamePlayData.OnHealthChanged -= OnHealthChanged;
		_gamePlayData.OnGoldChanged -= OnGoldChanged;
		_gamePlayData.OnRelicsChanged -= OnRelicsChanged;
	}

	private void Start() {
		_mapButton.onClick.AddListener(_mapOverlay.Toggle);
		_deckButton.onClick.AddListener(ToggleDeckList);
		_mapOverlay.GetComponent<MapRenderer>().Init();
		RefreshAll();
	}

	private void ToggleDeckList() {
		CardListOverlayController.Instance?.Toggle(GamePlayData.Instance.Deck, "덱 내의 카드 목록");
	}

	private void RefreshAll() {
		OnHealthChanged(_gamePlayData.CurrentHealth, _gamePlayData.MaxHealth);
		OnGoldChanged(_gamePlayData.Gold);
		OnRelicsChanged();
	}

	private void OnHealthChanged(int current, int max) {
		_hpText.text = $"{current}/{max}";
	}

	private void OnGoldChanged(int gold) {
		_goldText.text = $"{gold}G";
	}

	private void OnRelicsChanged() {
		ConfigureRelicRow();
		var hudFont = GetHudFont();

		foreach (Transform child in _relicRow) Destroy(child.gameObject);

		CreateRelicLabel(hudFont);
		foreach (var relic in _gamePlayData.Relics) {
			var icon = Instantiate(_relicIconPrefab, _relicRow);
			icon.Set(relic, hudFont);
		}
	}

	private TMP_FontAsset GetHudFont() {
		if (_relicFont != null) return _relicFont;
		if (_hpText != null && _hpText.font != null) return _hpText.font;
		if (_goldText != null && _goldText.font != null) return _goldText.font;
		return null;
	}

	private void ConfigureRelicRow() {
		var rectTransform = (RectTransform)_relicRow;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, RelicRowHeight);

		var layoutGroup = _relicRow.GetComponent<HorizontalLayoutGroup>();
		if (layoutGroup == null) return;

		layoutGroup.childAlignment = TextAnchor.MiddleLeft;
		layoutGroup.spacing = 8f;
		layoutGroup.childForceExpandWidth = false;
		layoutGroup.childForceExpandHeight = false;
		layoutGroup.childControlWidth = false;
		layoutGroup.childControlHeight = false;
	}

	private void CreateRelicLabel(TMP_FontAsset fontAsset) {
		var labelObject = new GameObject("RelicLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		labelObject.transform.SetParent(_relicRow, false);

		var labelRect = (RectTransform)labelObject.transform;
		labelRect.sizeDelta = new Vector2(RelicLabelWidth, RelicRowHeight);

		var layoutElement = labelObject.AddComponent<LayoutElement>();
		layoutElement.preferredWidth = RelicLabelWidth;
		layoutElement.preferredHeight = RelicRowHeight;

		var background = labelObject.GetComponent<Image>();
		background.color = new Color(0.13f, 0.1f, 0.07f, 0.9f);

		var outline = labelObject.AddComponent<Outline>();
		outline.effectColor = new Color(0.95f, 0.78f, 0.38f, 0.75f);
		outline.effectDistance = new Vector2(2f, -2f);

		var textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		textObject.transform.SetParent(labelObject.transform, false);

		var textRect = (RectTransform)textObject.transform;
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.sizeDelta = Vector2.zero;
		textRect.anchoredPosition = Vector2.zero;

		var text = textObject.GetComponent<TextMeshProUGUI>();
		text.text = "\uC720\uBB3C";
		if (fontAsset != null) text.font = fontAsset;
		text.alignment = TextAlignmentOptions.Center;
		text.color = new Color(0.96f, 0.9f, 0.72f, 1f);
		text.fontSize = 18f;
		text.fontStyle = FontStyles.Bold;
		text.raycastTarget = false;
	}
}
