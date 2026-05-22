using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLayoutController : MonoBehaviour {
	private float _spreadWidth;
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

	[Header("=== 카드 생성 시 이벤트 등록을 위함  ===")]
	[SerializeField] private BattleMouseController _battleMouseController;

	[Header("=== 각 카드별로 주입해주기 위함 ===")] 
	[SerializeField] private BattleManager _battleManager;
	
	private readonly List<CardOnHandController> _cards = new();
	
	private float _useCardDuration = 0.5f;
	
	/// <summary>
	/// Hand에서 카드 추가. 직접 호출하지 않고 DeckManager에 의해서만 호출되어야 함
	/// </summary>
	/// <param name="cardInstance"></param>
	public void AddCard(CardInstance cardInstance) {
		CardOnHandController cardController = _cardPool.GetCard(transform);
		_cards.Add(cardController);
		cardController.Init(cardInstance, _drawPileLocation, _discardPileLocation, _cardPool, _battleManager); 
		cardController.transform.SetAsLastSibling();
		cardController.gameObject.name = $"Card {_cards.Count}";
		
		// 생성 시 이벤트 등록
		_battleManager.OnCardUse.AddListener(cardController.RefreshCardDescription);
		_battleMouseController.OnTargetChange.AddListener(cardController.RefreshCardDescription);
		
		Arrange();
	}
	
	/// <summary>
	/// Hand에서 특정 카드를 삭제한다. DeckManager에 의해서만 호출되어야 함.
	/// 일반 카드 사용 로직에서는 직접 호출하지 않음
	/// </summary>
	/// <param name="card">삭제할 카드</param>
	public void RemoveCard(CardInstance card) {
		for (int i = 0; i < _cards.Count; i++) {
			if (_cards[i].CardInstance != card) continue;
			RemoveCard(_cards[i]);
			break;
		}
	}
	
	private void RemoveCard(CardOnHandController target) {
		int idx = _cards.IndexOf(target);
		// 이미 제거됐으면 조용히 종료
		if (idx < 0) return;             

		_battleManager.OnCardUse.RemoveListener(target.RefreshCardDescription);
		_battleMouseController.OnTargetChange.RemoveListener(target.RefreshCardDescription);

		target.RemoveCard();
		_cards.RemoveAt(idx);
		Arrange();
	}

	/// <summary>
	/// 카드 사용 시 카드 사용 위치에 카드를 잠깐 띄워줬다가, 카드 삭제 처리
	/// </summary>
	public void UseCard(CardInstance card) {
		for (int i = 0; i < _cards.Count; i++) {
			if (_cards[i].CardInstance != card) continue;
			StartCoroutine(CoUseCard(i));
			break;
		}
	}
	
	public IEnumerator CoUseCard(int idx) {
		// 참조 미리 캡처 - 추후 Index 기반 삭제 시에 문제 생김
		var cardInstance = _cards[idx].CardInstance;  
		_cards[idx].ToUsePosition();
		
		// 일정 시간 기다렸다 Instance 삭제
		yield return new WaitForSeconds(_useCardDuration);
		
		RemoveCard(cardInstance); 
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
			
			_cards[i].CardIdxInHand = i;
			_cards[i].transform.SetSiblingIndex(i);
		}
	}
}
