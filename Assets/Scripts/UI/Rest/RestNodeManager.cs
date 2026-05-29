using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestNodeManager : MonoBehaviour {
	[Header("=== 메인 패널 ===")]
	[SerializeField] private GameObject _mainPanel;
	[SerializeField] private Button _healButton;
	[SerializeField] private Button _burnButton;
	[SerializeField] private TextMeshProUGUI _healPreviewText;

	[Header("=== 카드 선택 패널 ===")]
	[SerializeField] private GameObject _cardSelectPanel;
	[SerializeField] private DeckRenderer _deckRenderer;
	[SerializeField] private Button _cardSelectCancelButton;

	[Header("=== 확인 팝업 ===")]
	[SerializeField] private GameObject _confirmPopup;
	[SerializeField] private TextMeshProUGUI _confirmText;
	[SerializeField] private Button _confirmButton;
	[SerializeField] private Button _confirmCancelButton;

	private CardInstance _pendingCard;
	private bool _completed;

	private void OnEnable() {
		_completed = false;
		var d = GamePlayData.Instance;
		int healAmount = Mathf.CeilToInt(d.MaxHealth * 0.30f);
		_healPreviewText.text = $"HP + {healAmount} 회복 (현재 {d.CurrentHealth} / {d.MaxHealth})";
		_burnButton.interactable = d.Deck.Count > 1;

		_healButton.onClick.AddListener(OnHealSelected);
		_burnButton.onClick.AddListener(OnBurnSelected);
		_cardSelectCancelButton.onClick.AddListener(ShowMainPanel);
		_confirmButton.onClick.AddListener(OnConfirmed);
		_confirmCancelButton.onClick.AddListener(OnConfirmCancelled);

		ShowMainPanel();
	}

	private void OnDisable() {
		_healButton.onClick.RemoveListener(OnHealSelected);
		_burnButton.onClick.RemoveListener(OnBurnSelected);
		_cardSelectCancelButton.onClick.RemoveListener(ShowMainPanel);
		_confirmButton.onClick.RemoveListener(OnConfirmed);
		_confirmCancelButton.onClick.RemoveListener(OnConfirmCancelled);
	}

	private void OnHealSelected() {
		if (_completed) return;
		LockButtons();
		var d = GamePlayData.Instance;
		d.SetHealth(d.CurrentHealth + Mathf.CeilToInt(d.MaxHealth * 0.30f));
		Complete();
	}

	private void OnBurnSelected() {
		_mainPanel.SetActive(false);
		_cardSelectPanel.SetActive(true);
		_deckRenderer.gameObject.SetActive(true);
		_deckRenderer.Init(GamePlayData.Instance.Deck, "제거할 카드를 선택하세요", OnCardPicked);
	}

	private void OnCardPicked(CardInstance card) {
		_pendingCard = card;
		_confirmText.text = $"[{card.CardName}]을(를) 영구 제거합니다. 계속하시겠습니까?";
		_cardSelectPanel.SetActive(false);
		_confirmPopup.SetActive(true);
	}

	private void OnConfirmed() {
		if (_completed) return;
		LockButtons();
		GamePlayData.Instance.RemoveCardFromDeck(_pendingCard);
		_pendingCard = null;
		Complete();
	}

	private void LockButtons() {
		_completed = true;
		_healButton.interactable = false;
		_burnButton.interactable = false;
	}

	private void OnConfirmCancelled() {
		_confirmPopup.SetActive(false);
		_cardSelectPanel.SetActive(true);
	}

	private void ShowMainPanel() {
		_mainPanel.SetActive(true);
		_cardSelectPanel.SetActive(false);
		_confirmPopup.SetActive(false);
	}

	private void Complete() {
		_mainPanel.SetActive(false);
		_cardSelectPanel.SetActive(false);
		_confirmPopup.SetActive(false);
		GameEvents.NodeCompleted();
	}
}
