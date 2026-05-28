using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 렌더링하고자 하는 List<CardInstance>를 받아서 카드 리스트를 렌더링한다.
/// </summary>
public class DeckRenderer : MonoBehaviour {
	[Header("=== 생성된 카드 인스턴스가 보일 곳 ===")]
	[SerializeField] private Transform _cardParent;
	[Header("=== 생성할 카드 Prefab ===")]
	[SerializeField] private CardViewController _cardPrefab;
	[Header("=== 설명할 텍스트 떠 있을 곳 ===")]
	[SerializeField] private TextMeshProUGUI _descriptionText;
	
	private List<CardViewController> _renderedCards = new List<CardViewController>();
	
	// 새로 띄워질 때마다 덱 리스트 렌더링
	// 눌렀을 때 수행할 함수까지 Init으로 받기
	public void Init(IEnumerable<CardInstance> cardInstances, string descriptionText, UnityAction<CardInstance> action) {
		Clear();
		_descriptionText.text = descriptionText;
		
		foreach (CardInstance cardInstance in cardInstances) {
			var card = Instantiate(_cardPrefab, _cardParent);
			_renderedCards.Add(card);
			
			card.Init(cardInstance, action);
		}
	}

	/// <summary>
	/// Disable 시에 모두 부수기
	/// </summary>
	public void Clear() {
		while (_renderedCards.Count > 0) {
			Destroy(_renderedCards[0].gameObject);
			_renderedCards.RemoveAt(0);
		}
	}

	private void OnDisable() {
		Clear();
	}
}
