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
		// 에너지 꽉 채우기
		_cardUseManager.StartTurn();
	}
	public void EndTurn() {
	}
	
	/// <summary>
	/// 카드 사용하고, 사용 성공/실패 여부를 Bool로 반환
	/// </summary>
	/// <param name="cardInstance">사용할 카드 instace</param>
	/// <param name="enemyInstance">대상 enemy</param>
	/// <returns></returns>
	public bool UseCard(CardInstance cardInstance, EnemyInstance enemyInstance) {
		// 1. 에너지 보고 카드 사용 가능한지 확인
		if (!_cardUseManager.isUsable(cardInstance)) {
			Debug.Log($"에너지가 부족합니다.");
			return false;
		}
		
		// 2. 대상이 필요한 카드인데, 대상이 없다면 사용 불가.
		if (cardInstance.NeedsTarget && enemyInstance == null) {
			Debug.Log($"대상이 필요합니다.");
			return false;
		}
		
		// 위의 사항에 해당 없다면, 카드 사용 처리
		_cardUseManager.UseCard(cardInstance);
		// 사용한 카드는 핸드에서 제거
		_deckManager.RemoveCardFromHand(cardInstance);
		return true;
	}
}	