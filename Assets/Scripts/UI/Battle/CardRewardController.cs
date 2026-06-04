using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardRewardController : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _goldAmountText;
	[SerializeField] private CardViewController[] _cardSlots;
	[SerializeField] private Button _skipButton;

	private UnityAction _onComplete;
	private CardViewController[] _runtimeSlots;

	public void Show(CardInstance[] rewardCards, int goldAmount, UnityAction onComplete) {
		_onComplete = onComplete;
		_goldAmountText.text = $"+ {goldAmount} 골드";
		gameObject.SetActive(true);
		EnsureSlotCount(rewardCards.Length);

		for (int i = 0; i < _runtimeSlots.Length; i++) {
			bool hasCard = i < rewardCards.Length;
			_runtimeSlots[i].gameObject.SetActive(hasCard);
			if (hasCard)
				_runtimeSlots[i].Init(rewardCards[i], OnCardSelected);
		}

		_skipButton.onClick.RemoveAllListeners();
		_skipButton.onClick.AddListener(Complete);
	}

	private void OnCardSelected(CardInstance selected) {
		GamePlayData.Instance.AddCardToDeck(selected._cardDefinition);
		Complete();
	}

	private void Complete() {
		gameObject.SetActive(false);
		_onComplete?.Invoke();
	}

	private void EnsureSlotCount(int count) {
		if (_runtimeSlots != null && _runtimeSlots.Length >= count) return;
		if (_cardSlots == null || _cardSlots.Length == 0) return;

		var slots = new CardViewController[count];
		for (int i = 0; i < count; i++) {
			if (i < _cardSlots.Length) {
				slots[i] = _cardSlots[i];
				continue;
			}

			slots[i] = Instantiate(_cardSlots[_cardSlots.Length - 1], _cardSlots[_cardSlots.Length - 1].transform.parent);
		}
		_runtimeSlots = slots;
	}
}
