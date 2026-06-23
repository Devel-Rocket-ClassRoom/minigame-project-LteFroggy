using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GamePlayData : Singleton<GamePlayData> {
	// 덱에 들어간 카드 목록
	public List<CardInstance> Deck { get; } = new();
	// 현재 게임의 맵
	public InGameMapData InGameMapData { get; private set; }

	// 가지고 있는 유물들 목록
	private readonly List<RelicBase> _relics = new();
	public IReadOnlyList<RelicBase> Relics => _relics;

	public event UnityAction<int, int> OnHealthChanged;
	public event UnityAction<int> OnGoldChanged;
	public event UnityAction OnRelicsChanged;
	public event UnityAction OnDeckChanged;

	[Header("=== 시작 카드들 등록 ===")]
	[SerializeField] private CardDefinition[] _startCards;

	[Header("=== 카드 보상 풀 (보상으로 등장 가능한 전체 카드 목록) ===")]
	[SerializeField] private CardDefinition[] _rewardCardPool;

	[Header("=== 플레이어의 최대 체력 ===")]
	[SerializeField] private int _maxHealth = 50;

	[Header("=== 플레이어의 초기 골드 ===")]
	[SerializeField] private int _startGold = 0;

	[Header("=== 맵 생성 정보 테이블 ===")]
	[SerializeField] private MapGeneratingConfig mapGeneratingConfig;
	public MapGeneratingConfig MapGeneratingConfig => mapGeneratingConfig;

	public int MaxHealth { get => _maxHealth; private set => _maxHealth = value; }
	public int CurrentHealth { get; private set; }
	public int Gold { get; private set; }

	protected override void Awake() {
		base.Awake();
		InitializeRunData();
	}

	// 런을 새로 시작하거나 메인 메뉴로 돌아올 때 호출 — 모든 런 데이터를 초기 상태로 되돌린다
	public void Reset() {
		InitializeRunData();
	}

	// 덱/체력/골드/유물/맵을 인스펙터 설정값 기준으로 초기화한다
	private void InitializeRunData() {
		CurrentHealth = _maxHealth;
		Gold = _startGold;

		Deck.Clear();
		foreach (CardDefinition card in _startCards)
			Deck.Add(new CardInstance(card));

		_relics.Clear();
		OnDeckChanged?.Invoke();
		OnRelicsChanged?.Invoke();
		OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
		
		InGameMapData = new InGameMapData();
	}

	public void SetHealth(int current) {
		CurrentHealth = Mathf.Clamp(current, 0, MaxHealth);
		OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
	}

	public void AddGold(int amount) {
		Gold += amount;
		OnGoldChanged?.Invoke(Gold);
	}

	public bool SpendGold(int amount) {
		if (Gold < amount) return false;
		Gold -= amount;
		OnGoldChanged?.Invoke(Gold);
		return true;
	}

	public void AddRelic(RelicBase relic) {
		_relics.Add(relic.CreateRuntimeCopy());
		OnRelicsChanged?.Invoke();
	}

	public void RemoveRelic(RelicBase relic) {
		_relics.Remove(relic);
		OnRelicsChanged?.Invoke();
	}

	public CardInstance[] GetRandomRewardCards(int count) {
		if (_rewardCardPool == null || _rewardCardPool.Length == 0) return Array.Empty<CardInstance>();

		var indices = new int[_rewardCardPool.Length];
		for (int i = 0; i < indices.Length; i++) indices[i] = i;
		for (int i = indices.Length - 1; i > 0; i--) {
			int j = Random.Range(0, i + 1);
			(indices[i], indices[j]) = (indices[j], indices[i]);
		}

		int take = Mathf.Min(count, _rewardCardPool.Length);
		var result = new CardInstance[take];
		for (int i = 0; i < take; i++) result[i] = new CardInstance(_rewardCardPool[indices[i]]);
		return result;
	}

	public CardInstance[] GetAllRewardCards() {
		if (_rewardCardPool == null || _rewardCardPool.Length == 0) return Array.Empty<CardInstance>();

		var seenCardIds = new HashSet<int>();
		var result = new List<CardInstance>();
		foreach (CardDefinition card in _rewardCardPool) {
			if (card == null || !seenCardIds.Add(card.cardId)) continue;
			result.Add(new CardInstance(card));
		}

		return result.ToArray();
	}

	public CardInstance GetRandomRewardCard(CardRarity rarity, IEnumerable<int> excludedCardIds) {
		if (_rewardCardPool == null || _rewardCardPool.Length == 0) return null;

		var excluded = new HashSet<int>(excludedCardIds);
		var candidates = new List<CardDefinition>();
		foreach (var card in _rewardCardPool) {
			if (card == null || card.rarity != rarity || excluded.Contains(card.cardId)) continue;
			candidates.Add(card);
		}

		if (candidates.Count == 0) return null;
		return new CardInstance(candidates[Random.Range(0, candidates.Count)]);
	}

	public void AddCardToDeck(CardDefinition definition) {
		Deck.Add(new CardInstance(definition));
		OnDeckChanged?.Invoke();
	}

	public void RemoveCardFromDeck(CardInstance card) {
		Deck.Remove(card);
		OnDeckChanged?.Invoke();
	}
}
