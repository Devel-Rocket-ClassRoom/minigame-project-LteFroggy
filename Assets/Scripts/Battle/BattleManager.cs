using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : BattleSystemManager {
	[SerializeField] private DeckManager _deckManager;
	[SerializeField] private CardUseManager _cardUseManager;
	[SerializeField] private EnemyManager _enemyManager;
	[SerializeField] private TurnManager _turnManager;
	[SerializeField] private CharacterManager _characterManager;
	[SerializeField] private BattleMouseController _mouseController;
	[SerializeField] private RelicManager _relicManager;

	[Header("=== 전투 종료 이후 상태 보여주는 임시 정보판 ===")] 
	[SerializeField] private GameObject _battleEndPanel;
	[SerializeField] private TextMeshProUGUI _battleEndText;
	[SerializeField] private TextMeshProUGUI _buttonText;
	[SerializeField] private Button _button;
	
	// 외부에서 드로우하려고 할 때 사용
	public DeckManager DeckManager => _deckManager;
	
	[Header("=== 턴 종료 버튼 ===")]
	[SerializeField] private Button _turnEndButton;
	
	// 카드 사용했을 때 발생시킬 이벤트
	[HideInInspector] public UnityEvent OnCardUse;
	
	private bool IsGameEnd;

	// Start에서 게임 시작
	private void Start() {
		StartBattle();
		
		// 전투 결과 패널 비활성화
		_battleEndPanel.SetActive(false);
	}

	public override void StartBattle() {
		_deckManager.StartBattle();
		_cardUseManager.StartBattle();
		_characterManager.StartBattle();
		_enemyManager.StartBattle();
		_turnManager.StartBattle();
		_relicManager.StartBattle();
		
		// 캐릭터 사망 시 게임오버되도록 함
		_characterManager.Player.OnDeath.AddListener(GameOver);
		
		// 적 모두 사망 시 스테이지 클리어
		_enemyManager.OnEnemyAllDead.AddListener(BattleEnd);
		
		StartPlayerTurn();
	}
	
	private void GameOver() {
		IsGameEnd = true;
		
		// 전투 결과 패널 활성화
		_battleEndPanel.SetActive(true);
		_battleEndText.text = "패배하였습니다";
		_buttonText.text = "다시 시도하기";
		
		// 패배 시에는 StartScene으로 돌려보내기.
		_button.onClick.AddListener(() => {
			// 돌려보내기 전에, 현재 씬에 살아있는 GamePlayData, UIBootstrapper 모두 삭제
			Destroy(GamePlayData.Instance.gameObject);
			Destroy(UISceneBootstrapper.Instance.gameObject);
			
			// StartScene으로 돌려보내기
			SceneManager.LoadScene(
				GamePlayData
					.Instance	
					.MapGeneratingConfig
					.GetConfig(MapNodeType.Start)
					.SceneName
			);
		});
		
		_characterManager.Player.OnDeath.RemoveListener(GameOver);
		_enemyManager.OnEnemyAllDead.RemoveListener(BattleEnd);
	}
	
	private void BattleEnd() {
		IsGameEnd = true;
		
		// 전투 종료되면, 현재 내 체력 저장
		GamePlayData.Instance.SetHealth(_characterManager.Player.CurrentHealth);
		
		_battleEndPanel.SetActive(true);
		_battleEndText.text = "전투 승리";
		_buttonText.text = "지도로 돌아가기";
		_button.onClick.AddListener(GameEvents.NodeCompleted);
		
		_characterManager.Player.OnDeath.RemoveListener(GameOver);
		_enemyManager.OnEnemyAllDead.RemoveListener(BattleEnd);
	}

	private void OnEnable() {
		_enemyManager.OnEnemyTurnEnd.AddListener(StartPlayerTurn);
	}

	private void OnDisable() {
		_enemyManager.OnEnemyTurnEnd.RemoveListener(StartPlayerTurn);
	}

	public override void StartPlayerTurn() {
		if (IsGameEnd) return;
		
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
		_cardUseManager.UseCard(cardInstance, GetCardUseContext(cardInstance));
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
	public CardUseContext GetCardUseContext(CardInstance cardInstance) {
		return new CardUseContext(
			this,
			_relicManager,
			_characterManager.Player,
			_enemyManager.EnemyList.Cast<CharacterBase>().ToList(),
			_mouseController.TargetInstance,
			cardInstance
		);
	}
}	