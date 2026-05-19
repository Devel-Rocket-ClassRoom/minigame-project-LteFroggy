using System;
using UnityEngine;

public class BattleManager : MonoBehaviour {
	[SerializeField] private DeckManager _deckManager;
	[SerializeField] private CardUseManager _cardUseManager;
	[SerializeField] private EnemyManager _enemyManager;
	
	private int _maxEnergy = 3;
	
	// 턴 관리
	private int _turnCount;
	private bool _isEnemyTurn;
	
	public int Turn => _turnCount;

	// Start에서 게임 시작
	private void Start() {
		_turnCount = 1;
		_deckManager.StartGame();
		StartTurn();
	}

	public void StartTurn() {
		_turnCount++;
		// 카드 5장 드로우
		for (int i = 0; i < 5; i++) {
			_deckManager.DrawCard();
		}
		
		_cardUseManager.StartTurn(_maxEnergy);
	}
	
	public void EndTurn() {
		
	}
}	