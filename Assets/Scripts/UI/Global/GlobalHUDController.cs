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

	private PlayerData _playerData;

	private void OnEnable() {
		_playerData = PlayerData.Instance;
		_playerData.HealthChanged += OnHealthChanged;
		_playerData.GoldChanged += OnGoldChanged;
		_playerData.RelicsChanged += OnRelicsChanged;
	}

	private void OnDisable() {
		if (_playerData == null) return;
		_playerData.HealthChanged -= OnHealthChanged;
		_playerData.GoldChanged -= OnGoldChanged;
		_playerData.RelicsChanged -= OnRelicsChanged;
	}

	private void Start() {
		_mapButton.onClick.AddListener(_mapOverlay.Open);
		_deckButton.onClick.AddListener(_deckOverlay.Open);
		RefreshAll();
	}

	private void RefreshAll() {
		OnHealthChanged(_playerData.CurrentHealth, _playerData.MaxHealth);
		OnGoldChanged(_playerData.Gold);
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
		foreach (var relic in _playerData.Relics) {
			var icon = Instantiate(_relicIconPrefab, _relicRow);
			icon.Set(relic);
		}
	}
}
