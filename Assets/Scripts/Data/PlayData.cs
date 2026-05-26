using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData> {
	// 덱에 들어간 카드 목록
	public List<CardInstance> Deck { get; } = new();

	// 가지고 있는 유물들 목록
	private readonly List<RelicBase> _relics = new();
	public IReadOnlyList<RelicBase> Relics => _relics;

	public event Action<int, int> HealthChanged;
	public event Action<int> GoldChanged;
	public event Action RelicsChanged;

	[Header("=== 시작 카드들 등록 ===")]
	[SerializeField] private CardDefinition[] _startCards;

	[Header("=== 플레이어의 최대 체력 ===")]
	[SerializeField] private int _maxHealth = 50;

	[Header("=== 플레이어의 초기 골드 ===")]
	[SerializeField] private int _startGold = 0;

	public int MaxHealth { get => _maxHealth; private set => _maxHealth = value; }
	public int CurrentHealth { get; private set; }
	public int Gold { get; private set; }

	protected override void Awake() {
		base.Awake();
		CurrentHealth = _maxHealth;
		Gold = _startGold;
		// 기본 덱 임시로 Awake에서 추가하도록 함.
		foreach (CardDefinition card in _startCards) {
			Deck.Add(new CardInstance(card));
		}
	}

	public void SetHealth(int current) {
		CurrentHealth = Mathf.Clamp(current, 0, MaxHealth);
		HealthChanged?.Invoke(CurrentHealth, MaxHealth);
	}

	public void AddGold(int amount) {
		Gold += amount;
		GoldChanged?.Invoke(Gold);
	}

	public bool SpendGold(int amount) {
		if (Gold < amount) return false;
		Gold -= amount;
		GoldChanged?.Invoke(Gold);
		return true;
	}

	public void AddRelic(RelicBase relic) {
		_relics.Add(relic);
		RelicsChanged?.Invoke();
	}

	public void RemoveRelic(RelicBase relic) {
		_relics.Remove(relic);
		RelicsChanged?.Invoke();
	}
}