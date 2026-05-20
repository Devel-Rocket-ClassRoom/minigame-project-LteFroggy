using System.Collections.Generic;
using UnityEngine;

public class HandLayoutController : MonoBehaviour {
	private float _spreadWidth;
	[SerializeField] private float _fanAngle = 20f;
	[SerializeField] private float _arcHeight = 200f;
	[SerializeField] private float _arcOffset = 50f;

	// 빌드 깨지지 않게 임시 주석처리. 이후 DrawPile, DiscardPile 완성되면 넣을 예정
	// [Header("=== 카드가 처음 드로우될 때 출발할 Position ===")]
	// [SerializeField] private RectTransform _drawPileLocation;
	//
	// [Header("=== 카드가 사용되고 들어갈 때 도착할 Position ===")]
	// [SerializeField] private RectTransform _discardPileLocation;

	[Header("=== Card Prefab 등록 ===")]
	[SerializeField] private CardOnHandController _cardPrefab;

	[Header("=== CardPool 등록 ===")]
	[SerializeField] private CardPool _cardPool;

	[Header("=== 각 카드별로 주입해주기 위함 ===")] 
	[SerializeField] private CardUseManager _cardUseManager;
	
	private readonly List<CardOnHandController> _cards = new();
	
	/// <summary>
	/// Hand에서 카드 추가. 직접 호출하지 않고 DeckManager에 의해서만 호출되어야 함
	/// </summary>
	/// <param name="cardInstance"></param>
	public void AddCard(CardInstance cardInstance) {
		CardOnHandController cardController = _cardPool.GetCard(transform);
		_cards.Add(cardController);
		cardController.Init(cardInstance, _cards.Count - 1);
		cardController.transform.SetAsLastSibling();
		
		// // 처음 뽑을 때 덱에서 나오는 것처럼 연출하기 위해 표시
		// cardController.SetCardPosition(_drawPileLocation.position, Quaternion.identity);
		
		Arrange();
	}

	/// <summary>
	/// Hand에서 카드 삭제. 직접 호출하지 않고 DeckManager에 의해서만 호출되어야 함
	/// 일반 카드 사용 로직에서는 직접 호출하지 않음
	/// </summary>
	/// <param name="card"></param>
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
		_spreadWidth = count * 150;
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
