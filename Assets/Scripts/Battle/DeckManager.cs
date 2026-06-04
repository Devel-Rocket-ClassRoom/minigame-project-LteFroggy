using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DeckManager : BattleSystemManager {

	[Header("=== 실제 UI카드상에 보이는 카드를 관리할 Controller ===")]
	[SerializeField] private HandLayoutController _handLayoutController;

	[Header("=== 뽑을 카드 더미, 사용한 카드 더미 텍스트 ===")]
	[SerializeField] private CardPileController _drawPileController;
	[SerializeField] private CardPileController _discardPileController;

	private readonly List<CardInstance> _drawPile = new();
	private readonly List<CardInstance> _discardPile = new();
	private readonly List<CardInstance> _exhaustPile = new();
	private readonly List<CardInstance> _handPile = new();
	private readonly List<CardInstance> _returnPile = new();   // 귀환: 다음 턴 손패로 돌아올 카드

	private bool _isFirstTurn;   // 선천 카드 보장용 (전투 첫 턴 여부)

	private readonly UnityEvent OnCardStateChanged = new();

	public int DrawCountOnNextTurn { get; set; } = 5;
	public bool BlockAdditionalDrawThisTurn { get; set; }
	public IReadOnlyList<CardInstance> HandPile => _handPile;

	// 전투 시작 시, PlayerData에서 덱 목록 가져오기
	public override void StartBattle() {
		foreach (var card in GamePlayData.Instance.Deck) {
			_discardPile.Add(card);
		}
		_isFirstTurn = true;

		_drawPileController.OnButtonPressed(ShowDrawPile);
		_discardPileController.OnButtonPressed(ShowDiscardPile);

		OnCardStateChanged.Invoke();
	}

	// 턴 시작되면, 귀환 카드 복귀 → 선천 카드 보장 → 나머지 드로우
	public override void StartPlayerTurn() {
		BlockAdditionalDrawThisTurn = false;

		// 귀환: 지난 턴 사용한 귀환 카드를 손패로 되돌림
		foreach (var card in _returnPile) {
			AddCardToHand(card);
		}
		_returnPile.Clear();

		// 선천: 첫 턴에는 선천 카드를 손패에 무조건 포함시킨 뒤, 남은 만큼만 드로우
		int drawCount = DrawCountOnNextTurn;
		if (_isFirstTurn) {
			drawCount -= DrawInnateCards();
			_isFirstTurn = false;
		}
		for (int i = 0; i < drawCount; i++) {
			DrawCard();
		}

		OnCardStateChanged.Invoke();
	}

	// 턴 종료되면, 유지 카드는 손에 남기고 나머지는 discardPile로
	public override void EndPlayerTurn() {
		foreach (var card in new List<CardInstance>(_handPile)) {
			if (card.Keyword.IsRetain) continue;   // 유지: 손에 남김
			RemoveCardFromHand(card);
		}

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 선천 카드를 덱에서 찾아 손패에 추가하고, 추가한 장수를 반환한다.
	/// </summary>
	private int DrawInnateCards() {
		if (_drawPile.Count == 0) { Shuffle(); }

		List<CardInstance> innates = new();
		foreach (var card in _drawPile) {
			if (card.Keyword.IsInnate) innates.Add(card);
		}
		foreach (var card in innates) {
			_drawPile.Remove(card);
			AddCardToHand(card);
		}
		return innates.Count;
	}

	private void OnEnable() {
		OnCardStateChanged.AddListener(UpdateCardText);
	}

	private void OnDisable() {
		_drawPile.Clear();
		_discardPile.Clear();
		_exhaustPile.Clear();
		_handPile.Clear();
		_returnPile.Clear();

		OnCardStateChanged.RemoveListener(UpdateCardText);
	}

	/// <summary>
	/// 카드 한 장 드로우
	/// </summary>
	public void DrawCard() {
		if (BlockAdditionalDrawThisTurn) return;
		if (_drawPile.Count == 0 && _discardPile.Count > 0) { Shuffle(); }
		if (_drawPile.Count == 0) return;

		_handPile.Add(_drawPile[_drawPile.Count - 1]);
		_drawPile.RemoveAt(_drawPile.Count - 1);
		_handLayoutController.AddCard(_handPile[_handPile.Count - 1]);

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 손에서 카드 제거
	/// </summary>
	/// <param name="card">제거할 카드</param>
	public void RemoveCardFromHand(CardInstance card) {
		_discardPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.RemoveCard(card);

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 손에서 사용한 카드 제거 (애니메이션 다름)
	/// </summary>
	/// <param name="card">사용한 카드</param>
	public void RemoveUsedCardFromHand(CardInstance card) {
		_discardPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.UseCard(card);

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 사용한 소멸(Exhaust) 카드를 손에서 제거 (이번 전투 동안 덱에 돌아오지 않음)
	/// </summary>
	/// <param name="card">사용한 소멸 카드</param>
	public void ExhaustUsedCardFromHand(CardInstance card) {
		_exhaustPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.UseCard(card);

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 사용한 귀환(Return) 카드를 손에서 제거하고 다음 턴 복귀 대기열에 넣는다.
	/// </summary>
	/// <param name="card">사용한 귀환 카드</param>
	public void ReturnUsedCardFromHand(CardInstance card) {
		_returnPile.Add(card);
		_handPile.Remove(card);
		_handLayoutController.UseCard(card);

		OnCardStateChanged.Invoke();
	}

	/// <summary>
	/// 이미 존재하는 카드 인스턴스를 손패에 추가한다 (드로우 더미를 거치지 않음).
	/// </summary>
	/// <param name="card">손패에 넣을 카드</param>
	private void AddCardToHand(CardInstance card) {
		_handPile.Add(card);
		_handLayoutController.AddCard(card);

		OnCardStateChanged.Invoke();
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

		OnCardStateChanged.Invoke();
	}

	private void ShowDrawPile() {
		CardListOverlayController.Instance?.Show(_drawPile, "뽑을 카드 더미");
	}

	private void ShowDiscardPile() {
		CardListOverlayController.Instance?.Show(_discardPile, "버린 카드 더미");
	}

	private void UpdateCardText() {
		_drawPileController.SetCountText(_drawPile.Count.ToString());
		_discardPileController.SetCountText(_discardPile.Count.ToString());
	}
}
