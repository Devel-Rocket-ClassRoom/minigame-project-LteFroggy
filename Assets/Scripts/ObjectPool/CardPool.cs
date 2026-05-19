using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour {
	[SerializeField] private CardOnHandController _cardPrefab;
	private readonly Stack<CardOnHandController> _cardPool = new();
	
	public CardOnHandController GetCard(Transform parent) {
		// 카드풀이 0이면, 새 카드 만들어주기
		CardOnHandController result;
		if (_cardPool.Count == 0) { result = Instantiate(_cardPrefab); }
		else { result = _cardPool.Pop(); }
		result.transform.SetParent(parent);
		result.transform.localScale = Vector3.one;
		result.gameObject.SetActive(true);
		return result;
	}
	
	public void ReturnCard(CardOnHandController card) {
		card.gameObject.SetActive(false);
		_cardPool.Push(card);
	}
}