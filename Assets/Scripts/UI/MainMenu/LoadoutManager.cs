using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour {
	[SerializeField] private Transform _relicListParent;
	[SerializeField] private GameObject _relicEntryPrefab;
	[SerializeField] private CardViewController _cardEntryPrefab;
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	[SerializeField] private TextMeshProUGUI _totalCostText;
	[SerializeField] private Button _startButton;
	[SerializeField] private int _costLimit = 6;
	[SerializeField] private int _cardPickCount = 2;

	private readonly HashSet<RelicBase> _selectedRelics = new();
	private readonly List<CardDefinition> _selectedCards = new();
	private readonly Dictionary<int, CardViewController> _cardViewsById = new();
	private int _totalCost;
	private LoadoutStep _step;

	private void OnEnable() {
		ShowRelicSelection();
		_startButton.onClick.AddListener(OnStartButtonClicked);
	}

	private void OnDisable() {
		_startButton.onClick.RemoveListener(OnStartButtonClicked);
	}

	private void ShowRelicSelection() {
		_step = LoadoutStep.RelicSelection;
		SetHeader("유물 선택", "코스트 합계 6 이내로 유물을 선택하세요");
		SetStartButtonText("카드 선택");
		_startButton.interactable = true;
		BuildRelicList();
	}

	private void BuildRelicList() {
		ClearSelectionList();

		_selectedRelics.Clear();
		_totalCost = 0;

		foreach (RelicBase relic in GamePlayData.AllLoadoutRelics) {
			RelicBase captured = relic;
			GameObject entry = Instantiate(_relicEntryPrefab, _relicListParent);

			SetChildText(entry, "RelicName", captured.displayName);
			SetChildText(entry, "EffectText", captured.effectDescription);
			SetChildText(entry, "CostBadge/CostValue", $"{captured.cost}");
			SetChildText(entry, "RarityBadge/RarityText", StringTableManager.StringTable[captured.rarity.ToString()]);
			SetRelicIcon(entry, captured.icon);

			var toggle = entry.GetComponent<Toggle>();
			toggle.SetIsOnWithoutNotify(false);
			toggle.onValueChanged.AddListener(isOn => OnRelicToggleChanged(captured, toggle, isOn));
		}

		UpdateCostDisplay();
	}

	private void ShowCardSelection() {
		_step = LoadoutStep.CardSelection;
		SetHeader("시작 카드 선택", "카드 보상 풀에서 서로 다른 카드 2장을 선택하세요");
		SetStartButtonText("출발");
		ClearSelectionList();
		_selectedCards.Clear();

		foreach (CardInstance card in GamePlayData.Instance.GetAllRewardCards()) {
			CardDefinition definition = card._cardDefinition;
			CardViewController view = Instantiate(_cardEntryPrefab, _relicListParent);
			_cardViewsById[definition.cardId] = view;
			view.Init(card, _ => ToggleStartCard(definition));
		}

		UpdateCardSelectionDisplay();
	}

	private static void SetChildText(GameObject root, string path, string text) {
		Transform t = root.transform.Find(path);
		if (t != null)
			t.GetComponent<TextMeshProUGUI>().text = text;
	}

	private static void SetRelicIcon(GameObject root, Sprite icon) {
		Transform iconBg = root.transform.Find("IconBg");
		if (iconBg == null) return;

		if (icon != null) {
			iconBg.GetComponent<Image>().sprite = icon;
			Transform placeholder = iconBg.Find("IconPlaceholder");
			if (placeholder != null) placeholder.gameObject.SetActive(false);
		}
	}

	private void OnRelicToggleChanged(RelicBase relic, Toggle toggle, bool isOn) {
		if (isOn) {
			if (_totalCost + relic.cost > _costLimit) {
				toggle.SetIsOnWithoutNotify(false);
				return;
			}

			_selectedRelics.Add(relic);
			_totalCost += relic.cost;
		} else {
			if (_selectedRelics.Remove(relic))
				_totalCost -= relic.cost;
		}

		SetHighlight(toggle.gameObject, isOn && _selectedRelics.Contains(relic));
		UpdateCostDisplay();
	}

	private static void SetHighlight(GameObject card, bool active) {
		Transform t = card.transform.Find("SelectHighlight");
		if (t != null) t.gameObject.SetActive(active);
	}

	private void ToggleStartCard(CardDefinition definition) {
		int selectedIndex = _selectedCards.FindIndex(card => card.cardId == definition.cardId);
		if (selectedIndex >= 0) {
			_selectedCards.RemoveAt(selectedIndex);
		} else {
			if (_selectedCards.Count >= _cardPickCount) return;
			_selectedCards.Add(definition);
		}

		UpdateCardSelectionDisplay();
	}

	private void UpdateCostDisplay() {
		_totalCostText.text = $"코스트 {_totalCost} / {_costLimit}";
	}

	private void UpdateCardSelectionDisplay() {
		_totalCostText.text = $"카드 {_selectedCards.Count} / {_cardPickCount}";
		_startButton.interactable = _selectedCards.Count == _cardPickCount;

		var selectedIds = new HashSet<int>();
		foreach (CardDefinition card in _selectedCards)
			selectedIds.Add(card.cardId);

		foreach (var pair in _cardViewsById) {
			bool selected = selectedIds.Contains(pair.Key);
			pair.Value.SetSelected(selected);
			pair.Value.SetInteractable(selected || _selectedCards.Count < _cardPickCount);
		}
	}

	private void OnStartButtonClicked() {
		if (_step == LoadoutStep.RelicSelection) {
			ShowCardSelection();
			return;
		}

		StartRun();
	}

	private void StartRun() {
		if (_selectedCards.Count != _cardPickCount) return;

		GamePlayData.Instance.Reset();
		foreach (RelicBase relic in _selectedRelics)
			GamePlayData.Instance.AddRelic(relic);
		foreach (CardDefinition card in _selectedCards)
			GamePlayData.Instance.AddCardToDeck(card);

		UISceneBootstrapper.Instance.TransitionTo(
			GamePlayData.Instance.MapGeneratingConfig
				.GetConfig(MapNodeType.Start).SceneName);
	}

	private void ClearSelectionList() {
		foreach (Transform child in _relicListParent)
			Destroy(child.gameObject);
		_cardViewsById.Clear();
	}

	private void SetHeader(string title, string description) {
		if (_titleText != null) _titleText.text = title;
		if (_descriptionText != null) _descriptionText.text = description;
	}

	private void SetStartButtonText(string text) {
		TextMeshProUGUI buttonText = _startButton.GetComponentInChildren<TextMeshProUGUI>(true);
		if (buttonText != null) buttonText.text = text;
	}

	private enum LoadoutStep {
		RelicSelection,
		CardSelection,
	}
}
