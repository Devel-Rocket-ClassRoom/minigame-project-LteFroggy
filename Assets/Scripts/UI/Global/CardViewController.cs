using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardViewController : MonoBehaviour {
	[Header("=== 세팅할 정보 UI ===")]
	[SerializeField] private Image _cardIcon;
	[SerializeField] private TextMeshProUGUI _cardNameText;
	[SerializeField] private TextMeshProUGUI _cardDescriptionText;
	[SerializeField] private TextMeshProUGUI _cardCostText;

	private Button _button;
	private CardInstance _cardInstance;

	/// <summary>
	/// CardView 초기화할 때 사용
	/// </summary>
	/// <param name="instance">담길 카드의 정보</param>
	/// <param name="action">카드가 클릭될 때 수행될 액션</param>
	public void Init(CardInstance instance, UnityAction<CardInstance> action = null) {
		_cardInstance = instance;
		
		_button = GetComponent<Button>();
		_button.onClick.RemoveAllListeners();
		if (action != null) _button.onClick.AddListener(() => action(_cardInstance));

		_cardIcon.sprite = _cardInstance.Icon;
		_cardNameText.text = _cardInstance.CardName;
		_cardDescriptionText.text = _cardInstance.GetCardDescription();
		_cardCostText.text = _cardInstance.Cost.ToString();
	}
}