using UnityEngine;

public class BattleManager : MonoBehaviour {
	[SerializeField] private DeckManager _deckManager;
	[SerializeField] private CardUseManager _cardUseManager;
	[SerializeField] private EnemyManager _enemyManager;
	[SerializeField] private CardUseManager cardUseManager;
	
	// 턴 관리
	private int _turnCount;
	private bool _isEnemyTurn;
	
	public int Turn => _turnCount;
	
	public void Init() {
		_turnCount = 0;
	}
	
	public void StartTurn() {
		_turnCount++;
		_deckManager.StartTurn();
		cardUseManager.StartTurn();
	}
}	