using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData> {
	private readonly List<CardInstance> _cards = new();
	public List<CardInstance> Cards => _cards;

	[Header("=== 시작 카드들 등록 ===")]
	[SerializeField] private CardDefinition[] _startCards;

	protected override void Awake() {
		base.Awake();
		
		// 기본 덱 임시로 Awake에서 추가하도록 함.
		foreach (CardDefinition card in _startCards) {
			_cards.Add(new CardInstance(card));
		}
	}
}