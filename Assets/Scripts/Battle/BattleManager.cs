using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : BattleSystemManager {
	[SerializeField] private DeckManager _deckManager;
	[SerializeField] private CardUseManager _cardUseManager;
	[SerializeField] private EnemyManager _enemyManager;
	[SerializeField] private TurnManager _turnManager;
	[SerializeField] private CharacterManager _characterManager;
	[SerializeField] private BattleMouseController _mouseController;
	
	// 외부에서 드로우하려고 할 때 사용
	public DeckManager DeckManager => _deckManager;
	
	[Header("=== 턴 종료 버튼 ===")]
	[SerializeField] private Button _turnEndButton;
	
	// 카드 사용했을 때 발생시킬 이벤트
	[HideInInspector] public UnityEvent OnCardUse;

	// Start에서 게임 시작
	private void Start() { StartBattle(); }

	public override void StartBattle() {
		_deckManager.StartBattle();
		_cardUseManager.StartBattle();
		_enemyManager.StartBattle();
		_turnManager.StartBattle();
		_characterManager.StartBattle();
		
		StartPlayerTurn();
	}
	
	public void DrawCard() {
		
	}

	private void OnEnable() {
		_enemyManager.OnEnemyTurnEnd.AddListener(StartPlayerTurn);
	}

	private void OnDisable() {
		_enemyManager.OnEnemyTurnEnd.RemoveListener(StartPlayerTurn);
	}

	public override void StartPlayerTurn() {
		_turnManager.StartPlayerTurn();
		_cardUseManager.StartPlayerTurn();
		_enemyManager.StartPlayerTurn();
		_deckManager.StartPlayerTurn();
		_characterManager.StartPlayerTurn();
	}
	
	public override void EndPlayerTurn() {
		_turnManager.EndPlayerTurn();
		_enemyManager.EndPlayerTurn();
		_cardUseManager.EndPlayerTurn();
		_deckManager.EndPlayerTurn();
		_characterManager.EndPlayerTurn();
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
		// 사용에 필요한 맥락 만들어서 주기
		_cardUseManager.UseCard(cardInstance, GetBattleContext());
		// 사용한 카드는 핸드에서 제거
		_deckManager.RemoveUsedCardFromHand(cardInstance);
		// 카드 사용했음 이벤트 발생
		OnCardUse?.Invoke();
		return true;
	}
	
	/// <summary>
	/// BattleContext 만들 때에는, _mouseController가 지정한 타겟 정보를 참조한다.
	/// </summary>
	/// <returns>만들어진 전투 맥락</returns>
	public BattleContext GetBattleContext() {
		return new BattleContext(
			this,
			_characterManager.Player,
			_enemyManager.EnemyList.Cast<CharacterBase>().ToList(),
			_mouseController.TargetInstance
		);
	}
}	