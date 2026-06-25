using System.Collections.Generic;
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
	[SerializeField] private Sprite _pauseIcon;
	[SerializeField] private TextMeshProUGUI _deckCountText;

	[Header("=== 유물 목록 ===")]
	[SerializeField] private Transform _relicRow;
	[SerializeField] private RelicIconController _relicIconPrefab;
	[SerializeField] private TMP_FontAsset _relicFont;

	[Header("=== 오버레이 패널 ===")]
	[SerializeField] private OverlayPanelController _mapOverlay;

	private const float RelicRowHeight = 88f;
	private const float RelicLabelWidth = 50f;
	private const string MapTooltipTitle = "지도";
	private const string MapTooltipDescription = "현재 지도의 진행 상황과 다음 선택 가능한 노드를 확인합니다.";
	private const string DeckTooltipTitle = "덱";
	private const string DeckTooltipDescription = "현재 덱의 카드 목록과 총 카드 수를 확인합니다.";

	private GamePlayData _gamePlayData;
	private PauseMenuController _pauseMenu;
	private int _openOverlayCount;
	private readonly Dictionary<string, RelicIconController> _relicIconsById = new();

	private void OnEnable() {
		_gamePlayData = GamePlayData.Instance;
		_gamePlayData.OnHealthChanged += OnHealthChanged;
		_gamePlayData.OnGoldChanged += OnGoldChanged;
		_gamePlayData.OnRelicsChanged += OnRelicsChanged;
		_gamePlayData.OnDeckChanged += RefreshDeckCount;

		GameEvents.OnNodeCompleted += OpenMapOverlay;
		GameEvents.OnNextNodeSelected += _mapOverlay.Close;
		GameEvents.OnRelicTriggered += OnRelicTriggered;

		OverlayPanelController.OnVisibilityChanged += OnOverlayVisibilityChanged;
	}

	private void OnDisable() {
		GameEvents.OnNodeCompleted -= OpenMapOverlay;
		GameEvents.OnNextNodeSelected -= _mapOverlay.Close;
		GameEvents.OnRelicTriggered -= OnRelicTriggered;

		OverlayPanelController.OnVisibilityChanged -= OnOverlayVisibilityChanged;
		_openOverlayCount = 0;

		if (_gamePlayData == null) return;
		_gamePlayData.OnHealthChanged -= OnHealthChanged;
		_gamePlayData.OnGoldChanged -= OnGoldChanged;
		_gamePlayData.OnRelicsChanged -= OnRelicsChanged;
		_gamePlayData.OnDeckChanged -= RefreshDeckCount;
	}

	private void Start() {
		ConfigureButtonTooltips();
		ConfigureDeckCountText();
		CardListOverlayController.Instance?.SetMutuallyExclusiveOverlay(_mapOverlay);
		_mapButton.onClick.AddListener(ToggleMapOverlay);
		_deckButton.onClick.AddListener(ToggleDeckList);
		_mapOverlay.GetComponent<MapRenderer>().Init();
		ConfigurePauseMenu();
		RefreshAll();
	}

	private void ToggleMapOverlay() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Close();
		_mapOverlay.Toggle();
	}

	private void OpenMapOverlay() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Close();
		_mapOverlay.Open();
	}

	private void ToggleDeckList() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Toggle(GamePlayData.Instance.Deck, "덱 내의 카드 목록");
	}

	private void RefreshAll() {
		OnHealthChanged(_gamePlayData.CurrentHealth, _gamePlayData.MaxHealth);
		OnGoldChanged(_gamePlayData.Gold);
		RefreshDeckCount();
		OnRelicsChanged();
	}

	private void OnHealthChanged(int current, int max) {
		_hpText.text = $"{current}/{max}";
	}

	private void OnGoldChanged(int gold) {
		_goldText.text = $"{gold}G";
	}

	private void RefreshDeckCount() {
		if (_deckCountText == null) return;

		_deckCountText.text = $"덱 {_gamePlayData.Deck.Count}";
	}

	private void ConfigureButtonTooltips() {
		EnsureTooltip(_mapButton, MapTooltipTitle, MapTooltipDescription);
		EnsureTooltip(_deckButton, DeckTooltipTitle, DeckTooltipDescription);
	}

	private static void EnsureTooltip(Button button, string title, string description) {
		if (button == null) return;

		var trigger = button.GetComponent<DescriptionTooltipTrigger>();
		if (trigger == null) trigger = button.gameObject.AddComponent<DescriptionTooltipTrigger>();
		trigger.SetContent(title, description);
	}

	private void ConfigureDeckCountText() {
		if (_deckCountText == null && _deckButton != null) {
			_deckCountText = _deckButton.GetComponentInChildren<TextMeshProUGUI>(true);
		}

		if (_deckCountText == null) return;

		_deckCountText.gameObject.SetActive(true);
		_deckCountText.raycastTarget = false;
		_deckCountText.alignment = TextAlignmentOptions.Center;
		_deckCountText.fontSize = 16f;
		_deckCountText.enableAutoSizing = true;
		_deckCountText.fontSizeMin = 10f;
		_deckCountText.fontSizeMax = 16f;
		_deckCountText.overflowMode = TextOverflowModes.Ellipsis;

		var textRect = (RectTransform)_deckCountText.transform;
		textRect.anchorMin = new Vector2(0f, 0f);
		textRect.anchorMax = new Vector2(1f, 0f);
		textRect.pivot = new Vector2(0.5f, 0f);
		textRect.anchoredPosition = new Vector2(0f, 4f);
		textRect.sizeDelta = new Vector2(0f, 24f);
	}

	private void ConfigurePauseMenu() {
		if (_pauseMenu == null) _pauseMenu = GetComponent<PauseMenuController>();
		if (_pauseMenu == null) _pauseMenu = gameObject.AddComponent<PauseMenuController>();

		_pauseMenu.Initialize(_mapButton, _deckButton, _mapOverlay, _pauseIcon, GetHudFont());
	}

	private void OnOverlayVisibilityChanged(bool isOpen) {
		_openOverlayCount = Mathf.Max(0, _openOverlayCount + (isOpen ? 1 : -1));
		_relicRow.gameObject.SetActive(_openOverlayCount == 0);
	}

	private void OnRelicsChanged() {
		ConfigureRelicRow();
		var hudFont = GetHudFont();
		_relicIconsById.Clear();

		foreach (Transform child in _relicRow) Destroy(child.gameObject);

		CreateRelicLabel(hudFont);
		foreach (var relic in _gamePlayData.Relics) {
			var icon = Instantiate(_relicIconPrefab, _relicRow);
			icon.Set(relic, hudFont);
			_relicIconsById[relic.relicId] = icon;
		}
	}

	private void OnRelicTriggered(RelicBase relic) {
		if (relic == null) return;
		if (_relicIconsById.TryGetValue(relic.relicId, out RelicIconController icon))
			icon.PlayTriggerFlash();
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
