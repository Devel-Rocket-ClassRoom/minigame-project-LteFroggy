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
	[SerializeField] private RelicManager _relicManager;

	[Header("=== 카드 보상 패널 ===")]
	[SerializeField] private CardRewardController _cardRewardController;
	[SerializeField] private int _goldReward = 20;

	[Header("=== 에너지 부족 안내 패널 ===")]
	[SerializeField] private InsufficientEnergyPanel _insufficientEnergyPanel;

	public DeckManager DeckManager => _deckManager;
	public RelicManager RelicManager => _relicManager;
	public CardUseManager CardUseManager => _cardUseManager;
	public PlayerCharacter Player => _characterManager.Player;

	[Header("=== 턴 종료 버튼 ===")]
	[SerializeField] private Button _turnEndButton;

	[Header("=== 임시 테스트 버튼 ===")]
	[SerializeField] private Button _cleareNodeButton;

	[HideInInspector] public UnityEvent OnCardUse;

	private bool IsGameEnd;
	private const string k_FinalEndingSceneName = "EndingScene";

	private void Start() {
		StartBattle();
	}

	public override void StartBattle() {
		_deckManager.SetBattleManager(this);
		_deckManager.StartBattle();
		_cardUseManager.StartBattle();
		_characterManager.StartBattle();
		_characterManager.Player.SetBattleManager(this);
		_enemyManager.StartBattle();
		_turnManager.StartBattle();
		_relicManager.StartBattle();
		_relicManager.OnBattleStart(this);

		_characterManager.Player.OnDeath.AddListener(GameOver);
		_enemyManager.OnEnemyAllDead.AddListener(BattleEnd);

		StartPlayerTurn();
	}

	private void GameOver() {
		if (IsGameEnd) return;

		IsGameEnd = true;
		StopBattleInteraction();
		RemoveBattleEndListeners();
		DefeatResultPanel.Show();
	}

	private void BattleEnd() {
		if (IsGameEnd) return;

		IsGameEnd = true;
		StopBattleInteraction();
		SetCleareNodeButtonInteractable(false);

		MapNodeType nodeType = GamePlayData.Instance.InGameMapData.NodeNow.Config.Type;
		GamePlayData.Instance.SetHealth(_characterManager.Player.CurrentHealth);
		int goldReward = _relicManager.ModifyGoldReward(nodeType, _goldReward);
		GamePlayData.Instance.AddGold(goldReward);

		if (nodeType == MapNodeType.Boss) {
			GameEvents.RunCleared();
			UISceneBootstrapper.Instance.TransitionTo(k_FinalEndingSceneName);
			RemoveBattleEndListeners();
			return;
		}

		int rewardCardCount = _relicManager.ModifyRewardCardCount(nodeType, 3);
		var rewardCards = new List<CardInstance>(GamePlayData.Instance.GetRandomRewardCards(rewardCardCount));
		_relicManager.ModifyRewardCards(nodeType, rewardCards);
		_cardRewardController.Show(rewardCards.ToArray(), goldReward, GameEvents.NodeCompleted);

		RemoveBattleEndListeners();
	}

	private void CompleteNodeForDebug() {
		if (IsGameEnd) return;

		SetCleareNodeButtonInteractable(false);
		BattleEnd();
	}

	private void RemoveBattleEndListeners() {
		_characterManager.Player.OnDeath.RemoveListener(GameOver);
		_enemyManager.OnEnemyAllDead.RemoveListener(BattleEnd);
	}

	private void SetCleareNodeButtonInteractable(bool interactable) {
		if (_cleareNodeButton == null) return;

		_cleareNodeButton.interactable = interactable;
	}

	private void StopBattleInteraction() {
		_mouseController?.StopBattleInteraction();
	}

	private void OnEnable() {
		_enemyManager.OnEnemyTurnEnd.AddListener(StartPlayerTurn);

		if (_cleareNodeButton != null)
			_cleareNodeButton.onClick.AddListener(CompleteNodeForDebug);
	}

	private void OnDisable() {
		_enemyManager.OnEnemyTurnEnd.RemoveListener(StartPlayerTurn);

		if (_cleareNodeButton != null)
			_cleareNodeButton.onClick.RemoveListener(CompleteNodeForDebug);
	}

	public override void StartPlayerTurn() {
		if (IsGameEnd) return;

		_turnManager.StartPlayerTurn();
		_cardUseManager.StartPlayerTurn(_characterManager.Player);
		_enemyManager.StartPlayerTurn();
		_characterManager.StartPlayerTurn();
		_relicManager.OnPlayerTurnStart(this, _turnManager.TurnCount);
		_deckManager.StartPlayerTurn();
	}

	public override void EndPlayerTurn() {
		_turnManager.EndPlayerTurn();
		_cardUseManager.EndPlayerTurn();
		_characterManager.EndPlayerTurn();
		_relicManager.OnPlayerTurnEnd(this, _turnManager.TurnCount);
		_deckManager.EndPlayerTurn();

		if (_relicManager.ConsumeSkipEnemyTurn()) {
			StartPlayerTurn();
			return;
		}

		_enemyManager.EndPlayerTurn();
	}

	public bool UseCard(CardInstance cardInstance, EnemyInstance enemyInstance) {
		if (!_deckManager.HandPile.Contains(cardInstance)) {
			Debug.Log("Card is no longer in hand.");
			return false;
		}

		if (_characterManager.Player.TryGetCardUseBlockedMessageKey(out string blockedMessageKey)) {
			_insufficientEnergyPanel?.Show(blockedMessageKey);
			Debug.Log("Status effect prevents card use.");
			return false;
		}

		if (!_cardUseManager.isUsable(cardInstance)) {
			_insufficientEnergyPanel?.Show();
			Debug.Log("에너지가 부족합니다.");
			return false;
		}

		if (!_characterManager.Player.CanUseCard()) {
			Debug.Log("이번 턴에는 더 이상 카드를 사용할 수 없습니다.");
			return false;
		}

		if (cardInstance.NeedsTarget && enemyInstance == null) {
			Debug.Log("대상이 필요합니다.");
			return false;
		}

		CardUseContext context = GetCardUseContext(cardInstance, enemyInstance);
		_relicManager.OnBeforeCardUse(context);
		_cardUseManager.UseCard(cardInstance, context);
		_characterManager.Player.NotifyCardUsed();
		_relicManager.OnAfterCardUse(context);

		if (context.ForceExhaustAfterUse || cardInstance.Keyword.IsExhaust)
			_deckManager.ExhaustUsedCardFromHand(cardInstance);
		else if (cardInstance.Keyword.IsReturn)
			_deckManager.ReturnUsedCardFromHand(cardInstance);
		else
			_deckManager.RemoveUsedCardFromHand(cardInstance);

		if (cardInstance.Keyword.IsChain)
			_deckManager.DrawCard(CardDrawSource.Keyword);

		OnCardUse?.Invoke();
		return true;
	}

	public CardUseContext GetCardUseContext(CardInstance cardInstance) {
		return GetCardUseContext(cardInstance, _mouseController.TargetInstance);
	}

	public CardUseContext GetCardUseContext(CardInstance cardInstance, EnemyInstance enemyInstance) {
		return new CardUseContext(
			this,
			_relicManager,
			_characterManager.Player,
			_enemyManager.EnemyList.Cast<CharacterBase>().ToList(),
			enemyInstance,
			cardInstance
		);
	}
}
