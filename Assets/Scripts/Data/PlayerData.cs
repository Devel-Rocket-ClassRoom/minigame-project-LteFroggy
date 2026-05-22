using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData> {
	// 덱에 들어간 카드 목록
	public List<CardInstance> Deck { get; } = new();

	// 가지고 있는 유물들 목록
	public List<RelicBase> Relics { get; } = new();

	[Header("=== 시작 카드들 등록 ===")]
	[SerializeField] private CardDefinition[] _startCards;

	[Header("=== 플레이어의 최대 체력 ===")]
	[SerializeField] private int _maxHealth = 50;
	public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
	public int CurrentHealth { get; set; }

	protected override void Awake() {
		base.Awake();
		
		CurrentHealth = _maxHealth;
		// 기본 덱 임시로 Awake에서 추가하도록 함.
		foreach (CardDefinition card in _startCards) {
			Deck.Add(new CardInstance(card));
		}
	}
}