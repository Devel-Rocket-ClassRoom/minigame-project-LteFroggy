using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

	[Header("=== 시작 카드들 등록 ===")]
	[SerializeField] private CardDefinition[] _startCards;

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
		
		CurrentHealth = _maxHealth;
		Gold = _startGold;
		
		// 기본 덱 임시로 Awake에서 추가하도록 함.
		foreach (CardDefinition card in _startCards) {
			Deck.Add(new CardInstance(card));
		}
		
		// 맵 노드 초기화
		MapGenerator.MapConfig = mapGeneratingConfig;
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
		_relics.Add(relic);
		OnRelicsChanged?.Invoke();
	}

	public void RemoveRelic(RelicBase relic) {
		_relics.Remove(relic);
		OnRelicsChanged?.Invoke();
	}
}