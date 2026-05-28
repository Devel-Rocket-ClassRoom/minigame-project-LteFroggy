using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardRewardController : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _goldAmountText;
	[SerializeField] private CardViewController[] _cardSlots;
	[SerializeField] private Button _skipButton;

	private UnityAction _onComplete;

	public void Show(CardInstance[] rewardCards, int goldAmount, UnityAction onComplete) {
		_onComplete = onComplete;
		_goldAmountText.text = $"+ {goldAmount} 골드";
		gameObject.SetActive(true);

		for (int i = 0; i < _cardSlots.Length; i++) {
			bool hasCard = i < rewardCards.Length;
			_cardSlots[i].gameObject.SetActive(hasCard);
			if (hasCard)
				_cardSlots[i].Init(rewardCards[i], OnCardSelected);
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
}
