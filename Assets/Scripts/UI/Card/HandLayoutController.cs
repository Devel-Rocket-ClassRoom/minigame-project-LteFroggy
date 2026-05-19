using System.Collections.Generic;
using UnityEngine;

public class HandLayoutController : MonoBehaviour {
	[SerializeField] private float _spreadWidth = 600f;
	[SerializeField] private float _fanAngle = 20f;
	[SerializeField] private float _arcHeight = 200f;
	[SerializeField] private float _arcOffset = 50f;

	[Header("=== 카드가 처음 드로우될 때 출발할 Position ===")]
	[SerializeField] private RectTransform _drawPileLocation;

	[Header("=== 카드가 사용되고 들어갈 때 도착할 Position ===")]
	[SerializeField] private RectTransform _discardPileLocation;

	[Header("=== Card Prefab 등록 ===")]
	[SerializeField] private CardOnHandController _cardPrefab;

	[Header("=== CardPool 등록 ===")]
	[SerializeField] private CardPool _cardPool;
	
	private readonly List<CardOnHandController> _cards = new();
	
	public void AddCard(CardInstance cardInstance) {
		CardOnHandController cardController = _cardPool.GetCard(transform);
		_cards.Add(cardController);
		cardController.Init(cardInstance, _cards.Count - 1);
		
		// 처음 뽑을 때 덱에서 나오는 것처럼 연출하기 위해 표시
		cardController.SetCardPosition(_drawPileLocation.position, Quaternion.identity);
		
		Arrange();
	}

	public void RemoveCard(CardInstance card) {
		for (int i = 0; i < _cards.Count; i++) {
			if (_cards[i].CardInstance != card) continue;
			_cardPool.ReturnCard(_cards[i]);
			_cards.RemoveAt(i);
			break;
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
