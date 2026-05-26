using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DeckManager : BattleSystemManager {

	[Header("=== 실제 UI카드상에 보이는 카드를 관리할 Controller ===")]
	[SerializeField] private HandLayoutController _handLayoutController;

	[Header("=== 뽑을 카드 더미, 사용한 카드 더미 텍스트 ===")] 
	[SerializeField] private TextMeshProUGUI _drawPileText;
	[SerializeField] private TextMeshProUGUI _discardPileText;
	
	private readonly List<CardInstance> _drawPile = new();
	private readonly List<CardInstance> _discardPile = new();
	private readonly List<CardInstance> _exhaustPile = new();
	private readonly List<CardInstance> _handPile = new();
	
	public int DrawCountOnNextTurn { get; set; } = 5;
	
	// 전투 시작 시, PlayerData에서 덱 목록 가져오기
	public override void StartBattle() {
		foreach (var card in PlayData.Instance.Deck) {
			_discardPile.Add(card);
		}
	}
	
	// 턴 시작되면, 카드 설정된 만큼 드로우 (기본 5장)
	public override void StartPlayerTurn() {
		for (int i = 0; i < DrawCountOnNextTurn; i++) {
			DrawCard();
		}
	}
	
	// 턴 종료되면, 손에 있는 카드 모두 discardPile로
	public override void EndPlayerTurn() {
		while (_handPile.Count > 0) {
			RemoveCardFromHand(_handPile[0]);
		}
	}

	private void OnDisable() {
		_drawPile.Clear();
		_discardPile.Clear();
		_exhaustPile.Clear();
		_handPile.Clear();
	}

	/// <summary>
	/// 카드 한 장 드로우
	/// </summary>
	public void DrawCard() {
		if (_drawPile.Count == 0) { Shuffle(); }
		_handPile.Add(_drawPile[_drawPile.Count - 1]);
		_drawPile.RemoveAt(_drawPile.Count - 1);
		_handLayoutController.AddCard(_handPile[_handPile.Count - 1]);
	}
	
	/// <summary>
	/// 손에서 카드 제거
	/// </summary>
	/// <param name="card">제거할 카드</param>
	public void RemoveCardFromHand(CardInstance card) { 
		_discardPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.RemoveCard(card);
	}
	
	/// <summary>
	/// 손에서 사용한 카드 제거 (애니메이션 다름)
	/// </summary>
	/// <param name="card">사용한 카드</param>
	public void RemoveUsedCardFromHand(CardInstance card) {
		_discardPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.UseCard(card);
	}
	
	/// <summary>
	/// 카드 부족할 시, 카드를 섞는다.
	/// </summary>
	private void Shuffle() {
		_drawPile.Clear();
		
		List<List<int>> temp = new List<List<int>>(_discardPile.Count);
		
		// 정렬 기준으로 0 ~ 100까지의 값 랜덤 할당
		for (int i = 0; i < _discardPile.Count; i++) {
			temp.Add(new List<int>());
			temp[i].Add(i);
			temp[i].Add(Random.Range(0, 100));
		}
		// 정렬 기준값 기반으로 Sort
		temp.Sort((listA, listB) => listA[1].CompareTo(listB[1]));
		
		// 이 기준으로 _drawPile에 삽입
		foreach (var sorted in temp) {
			_drawPile.Add(_discardPile[sorted[0]]);
		}
		// 버려진 카드 목록 초기화
		_discardPile.Clear();
	}

	private void Update() {
		_drawPileText.text = _drawPile.Count.ToString();
		_discardPileText.text = _discardPile.Count.ToString();
	}
}