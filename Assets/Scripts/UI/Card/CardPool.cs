using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour {
	[SerializeField] private CardOnHandController _cardPrefab;
	private readonly Stack<CardOnHandController> _cardPool = new();
	
	public CardOnHandController GetCard(Transform parent) {
		// 카드풀이 0이면, 새 카드 만들어주기
		if (_cardPool.Count == 0) {
			var instance = Instantiate(_cardPrefab);
			return instance;
		}
		
		var controller = _cardPool.Pop();
		controller.transform.parent = parent;
		controller.gameObject.SetActive(true);
		return controller;
	}
	
	public void ReturnCard(CardOnHandController card) {
		card.gameObject.SetActive(false);
		_cardPool.Push(card);
	}
}