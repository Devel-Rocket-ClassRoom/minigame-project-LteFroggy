using System.Collections.Generic;
using UnityEngine;

public class HandLayoutController : MonoBehaviour {
	[SerializeField] private float _spreadWidth = 600f;
	[SerializeField] private float _fanAngle = 20f;
	[SerializeField] private float _arcHeight = 200f;
	[SerializeField] private float _arcOffset = 50f;

	[Header("=== Card Prefab 등록 ===")]
	[SerializeField] private CardOnHandController _cardPrefab;

	[Header("=== CardPool 등록 ===")]
	[SerializeField] private CardPool _cardPool;
	
	private List<CardOnHandController> _cards = new();
	
	public void AddCard(CardInstance cardInstance) {
		CardOnHandController cardController = _cardPool.GetCard(transform);
		_cards.Add(cardController);
		cardController.Init(cardInstance, _cards.Count - 1);
		Arrange();
	}

	public void RemoveCard(CardInstance card) {
		for (int i = 0; i < _cards.Count; i++) {
			if (_cards[i].CardInstance == card) {
				_cards.RemoveAt(i);
				_cardPool.ReturnCard(_cards[i]);
			}
		}
		Arrange();
	}

	public void Arrange() {
		int count = _cards.Count;
		for (int i = 0; i < count; i++) {
			// t: -0.5(맨 왼쪽) ~ 0(가운데) ~ 0.5(맨 오른쪽)로 정규화
			float t = count == 1 ? 0f : (i / (float)(count - 1)) - 0.5f;

			// 가운데 기준으로 좌우로 펼침
			float x = t * _spreadWidth;

			// 포물선 형태로 가운데가 가장 낮고 양쪽이 올라감 (손에 쥔 느낌)
			float y = -(t * t) * _arcHeight + _arcOffset;

			// 가운데는 수직, 양쪽으로 갈수록 바깥 방향으로 기울어짐
			float rot = -t * _fanAngle;

			_cards[i].SetFanTransform(
				new Vector3(x, y, 0),
				Quaternion.Euler(0, 0, rot)
			);
		}
	}
}
