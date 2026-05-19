using UnityEngine;

public class BattleManager : MonoBehaviour {
	[SerializeField] private DeckManager _deckManager;
	[SerializeField] private CardUseManager _cardUseManager;
	[SerializeField] private EnemyManager _enemyManager;
	
	// 턴 관리
	private int _turnCount;
	private bool _isEnemyTurn;
	
	public int Turn => _turnCount;
	
	public void Init() {
		_turnCount = 0;
	}
	
	public void StartTurn() {
		_turnCount++;
		// 카드 5장 드로우
		for (int i = 0; i < 5; i++) {
			_deckManager.DrawCard();
			_cardUseManager.StartTurn();
		}
	}
	
	public void EndTurn() {
		
	}
}	