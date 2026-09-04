using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DeckManager : BattleSystemManager {
	private const string DrawPileTooltipTitle = "뽑을 카드 더미";
	private const string DrawPileTooltipDescription = "앞으로 뽑을 카드가 들어 있는 더미입니다. 클릭하면 카드 목록을 확인합니다.";
	private const string DiscardPileTooltipTitle = "버린 카드 더미";
	private const string DiscardPileTooltipDescription = "사용했거나 버려진 카드가 모이는 더미입니다. 클릭하면 카드 목록을 확인합니다.";
	private const string ExhaustPileTooltipTitle = "소멸된 카드 더미";
	private const string ExhaustPileTooltipDescription = "이번 전투 동안 덱으로 돌아오지 않는 소멸 카드가 모이는 더미입니다. 클릭하면 카드 목록을 확인합니다.";

	[Header("=== 실제 UI카드상에 보이는 카드를 관리할 Controller ===")]
	[SerializeField] private HandLayoutController _handLayoutController;

	[Header("=== 뽑을 카드 더미, 사용한 카드 더미 텍스트 ===")]
	[SerializeField] private CardPileController _drawPileController;
	[SerializeField] private CardPileController _discardPileController;
	[SerializeField] private CardPileController _exhaustPileController;
	[SerializeField] private Vector2 _runtimeExhaustPileOffset = new(-120f, 0f);
	[SerializeField] private DeckShuffleAnimator _deckShuffleAnimator;

	private readonly List<CardInstance> _drawPile = new();
	private readonly List<CardInstance> _discardPile = new();
	private readonly List<CardInstance> _exhaustPile = new();
	private readonly List<CardInstance> _handPile = new();
	private readonly Queue<CardDrawSource> _pendingDraws = new();
	private readonly List<CardInstance> _returnPile = new();   // 귀환: 다음 턴 손패로 돌아올 카드

	private bool _isFirstTurn;   // 선천 카드 보장용 (전투 첫 턴 여부)

	private Coroutine _drawQueueCoroutine;
	private Coroutine _turnStartDrawCoroutine;

	private readonly UnityEvent OnCardStateChanged = new();
	private BattleManager _battleManager;

	public int DrawCountOnNextTurn { get; set; } = 5;
	public bool BlockAdditionalDrawThisTurn { get; set; }
	public IReadOnlyList<CardInstance> HandPile => _handPile;
	public IReadOnlyList<CardInstance> ExhaustPile => _exhaustPile;

	public void SetBattleManager(BattleManager battleManager) {
		_battleManager = battleManager;
	}

	// 전투 시작 시, PlayerData에서 덱 목록 가져오기
	public override void StartBattle() {
		DrawCountOnNextTurn = 5;
		BlockAdditionalDrawThisTurn = false;
		foreach (var card in GamePlayData.Instance.Deck) {
			card.ResetBattleModifiers();
			_discardPile.Add(card);
		}
		_isFirstTurn = true;

		EnsureExhaustPileController();
		_drawPileController?.OnButtonPressed(ShowDrawPile);
		_discardPileController?.OnButtonPressed(ShowDiscardPile);
		_exhaustPileController?.OnButtonPressed(ShowExhaustPile);
		_drawPileController?.SetTooltip(DrawPileTooltipTitle, DrawPileTooltipDescription);
		_discardPileController?.SetTooltip(DiscardPileTooltipTitle, DiscardPileTooltipDescription);
		_exhaustPileController?.SetTooltip(ExhaustPileTooltipTitle, ExhaustPileTooltipDescription);

		OnCardStateChanged.Invoke();
	}

	// 턴 시작되면, 귀환 카드 복귀 -> 선천 카드 보장 -> 나머지 드로우
	public override void StartPlayerTurn() {
		BlockAdditionalDrawThisTurn = false;
		ResetTurnModifiers();

		// 귀환: 지난 턴 사용한 귀환 카드를 손패로 되돌림
		foreach (var card in _returnPile) {
			AddCardToHand(card);
			_battleManager?.RelicManager.OnReturnedCardToHand(_battleManager, card);
		}
		_returnPile.Clear();

		// 선천: 첫 턴에는 선천 카드를 손패에 무조건 포함시킨 뒤, 남은 만큼만 드로우
		if (_turnStartDrawCoroutine != null) {
			StopCoroutine(_turnStartDrawCoroutine);
		}
		_turnStartDrawCoroutine = StartCoroutine(CoDrawTurnStartCards());
	}

	private IEnumerator CoDrawTurnStartCards() {
		int drawCount = DrawCountOnNextTurn;
		if (_isFirstTurn) {
			if (_drawPile.Count == 0 && _discardPile.Count > 0) {
				yield return CoShuffle();
			}
			drawCount -= DrawInnateCards();
			_isFirstTurn = false;
		}
		for (int i = 0; i < drawCount; i++) {
			DrawCard(CardDrawSource.TurnStart);
		}

		OnCardStateChanged.Invoke();
		_turnStartDrawCoroutine = null;
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
		if (_turnStartDrawCoroutine != null) {
			StopCoroutine(_turnStartDrawCoroutine);
			_turnStartDrawCoroutine = null;
		}
		if (_drawQueueCoroutine != null) {
			StopCoroutine(_drawQueueCoroutine);
			_drawQueueCoroutine = null;
		}

		_drawPile.Clear();
		_discardPile.Clear();
		_exhaustPile.Clear();
		_handPile.Clear();
		_returnPile.Clear();
		_pendingDraws.Clear();

		OnCardStateChanged.RemoveListener(UpdateCardText);
	}

	/// <summary>
	/// 카드 한 장 드로우
	/// </summary>
	public void DrawCard(CardDrawSource source = CardDrawSource.CardEffect) {
		if (BlockAdditionalDrawThisTurn) return;

		if (_drawQueueCoroutine == null && _pendingDraws.Count == 0 && _drawPile.Count > 0) {
			DrawTopCard(source);
			return;
		}

		_pendingDraws.Enqueue(source);
		if (_drawQueueCoroutine == null) {
			_drawQueueCoroutine = StartCoroutine(CoProcessDrawQueue());
		}
	}

	private IEnumerator CoProcessDrawQueue() {
		while (_pendingDraws.Count > 0) {
			CardDrawSource source = _pendingDraws.Dequeue();
			if (_drawPile.Count == 0 && _discardPile.Count > 0) {
				yield return CoShuffle();
			}
			if (_drawPile.Count == 0) continue;

			DrawTopCard(source);
		}

		_drawQueueCoroutine = null;
	}

	private void DrawTopCard(CardDrawSource source) {
		CardInstance drawn = _drawPile[_drawPile.Count - 1];
		_handPile.Add(drawn);
		_drawPile.RemoveAt(_drawPile.Count - 1);
		_handLayoutController.AddCard(drawn);
		_battleManager?.RelicManager.OnCardDrawn(_battleManager, drawn, source);

		OnCardStateChanged.Invoke();
	}

	public void AddNextTurnDrawBonus(int amount) {
		DrawCountOnNextTurn += amount;
	}

	public void AddPlayerBlock(int amount) {
		_battleManager?.Player?.AddBlock(amount);
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

	private void ResetTurnModifiers() {
		foreach (var card in _drawPile) card.ResetTurnModifiers();
		foreach (var card in _discardPile) card.ResetTurnModifiers();
		foreach (var card in _exhaustPile) card.ResetTurnModifiers();
		foreach (var card in _handPile) card.ResetTurnModifiers();
		foreach (var card in _returnPile) card.ResetTurnModifiers();
	}

	/// <summary>
	/// 카드 부족할 시, 카드를 섞는다.
	/// </summary>
	private IEnumerator CoShuffle() {
		int shuffleCount = ShuffleDiscardPileIntoDrawPile();
		if (shuffleCount == 0) yield break;

		OnCardStateChanged.Invoke();
		if (_deckShuffleAnimator == null) yield break;

		yield return _deckShuffleAnimator.PlayShuffle(
			_discardPileController != null ? _discardPileController.RectTransform : null,
			_drawPileController != null ? _drawPileController.RectTransform : null,
			shuffleCount
		);

	}

	private int ShuffleDiscardPileIntoDrawPile() {
		if (_discardPile.Count == 0) return 0;

		List<CardInstance> shuffledCards = new(_discardPile);
		for (int i = 0; i < shuffledCards.Count; i++) {
			int randomIndex = Random.Range(i, shuffledCards.Count);
			CardInstance card = shuffledCards[i];
			shuffledCards[i] = shuffledCards[randomIndex];
			shuffledCards[randomIndex] = card;
		}

		foreach (var card in shuffledCards) {
			_drawPile.Add(card);
		}
		foreach (var card in shuffledCards) {
			_discardPile.Remove(card);
		}

		return shuffledCards.Count;
	}

	private void ShowDrawPile() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Show(_drawPile, "뽑을 카드 더미");
	}

	private void ShowDiscardPile() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Show(_discardPile, "버린 카드 더미");
	}

	private void ShowExhaustPile() {
		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Show(_exhaustPile, "소멸된 카드 더미");
	}

	private void UpdateCardText() {
		_drawPileController?.SetCountText(_drawPile.Count.ToString());
		_discardPileController?.SetCountText(_discardPile.Count.ToString());
		_exhaustPileController?.SetCountText(_exhaustPile.Count.ToString());
	}

	private void EnsureExhaustPileController() {
		if (_exhaustPileController != null || _discardPileController == null)
			return;

		_exhaustPileController = Instantiate(_discardPileController, _discardPileController.transform.parent);
		_exhaustPileController.gameObject.name = "ExhaustPileUI";

		RectTransform rectTransform = _exhaustPileController.RectTransform;
		if (rectTransform != null)
			rectTransform.anchoredPosition += _runtimeExhaustPileOffset;
		if (rectTransform != null) {
			rectTransform.anchorMin = Vector2.one;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.pivot = Vector2.one;
			rectTransform.anchoredPosition = new Vector2(-36f, -184f);
			rectTransform.sizeDelta = new Vector2(104f, 104f);
		}

		Image image = _exhaustPileController.GetComponentInChildren<Image>();
		if (image != null)
			image.color = new Color(0.32f, 0.18f, 0.38f, image.color.a);
		if (image != null) {
			Sprite exhaustPileSprite = Resources.Load<Sprite>("Sprites/UI/ExhaustPile");
			if (exhaustPileSprite != null) {
				image.sprite = exhaustPileSprite;
				image.preserveAspect = true;
			}
			else {
				Debug.LogError("[DeckManager] 소멸 카드 더미 스프라이트를 찾을 수 없습니다: Resources/Sprites/UI/ExhaustPile");
			}

			image.color = Color.white;
		}
	}
}
