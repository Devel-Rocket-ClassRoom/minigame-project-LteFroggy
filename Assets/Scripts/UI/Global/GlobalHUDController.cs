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

	[Header("=== 오버레이 패널 ===")]
	[SerializeField] private OverlayPanelController _mapOverlay;
	[SerializeField] private OverlayPanelController _deckOverlay;

	private PlayData _playData;

	private void OnEnable() {
		_playData = PlayData.Instance;
		_playData.OnHealthChanged += OnHealthChanged;
		_playData.OnGoldChanged += OnGoldChanged;
		_playData.OnRelicsChanged += OnRelicsChanged;
	}

	private void OnDisable() {
		if (_playData == null) return;
		_playData.OnHealthChanged -= OnHealthChanged;
		_playData.OnGoldChanged -= OnGoldChanged;
		_playData.OnRelicsChanged -= OnRelicsChanged;
	}

	private void Start() {
		_mapButton.onClick.AddListener(_mapOverlay.Open);
		_deckButton.onClick.AddListener(_deckOverlay.Open);
		RefreshAll();
	}

	private void RefreshAll() {
		OnHealthChanged(_playData.CurrentHealth, _playData.MaxHealth);
		OnGoldChanged(_playData.Gold);
		OnRelicsChanged();
	}

	private void OnHealthChanged(int current, int max) {
		_hpText.text = $"{current}/{max}";
	}

	private void OnGoldChanged(int gold) {
		_goldText.text = $"{gold}";
	}

	private void OnRelicsChanged() {
		foreach (Transform child in _relicRow) Destroy(child.gameObject);
		foreach (var relic in _playData.Relics) {
			var icon = Instantiate(_relicIconPrefab, _relicRow);
			icon.Set(relic);
		}
	}
}
