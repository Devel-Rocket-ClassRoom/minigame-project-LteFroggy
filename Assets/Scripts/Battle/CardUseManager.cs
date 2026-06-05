using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CardUseManager : BattleSystemManager {
	private int _maxEnergy = 3;
	private int _currentEnergy;
	public int CurrentEnergy => _currentEnergy;

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;

	[HideInInspector] public UnityEvent<int> OnEnergyChanged = new();

	public override void StartPlayerTurn() {
		SetCurrentEnergy(_maxEnergy);
	}

	public void StartPlayerTurn(CharacterBase player) {
		if (player == null) {
			StartPlayerTurn();
			return;
		}

		SetCurrentEnergy(Mathf.Max(0, player.CalculateStartingEnergy(_maxEnergy)));
	}
	
	/// <summary>
	/// 특정 카드 사용 가능 여부 확인
	/// </summary>
	/// <param name="instance">판별할 카드</param>
	/// <returns>사용 가능 여부</returns>
	public bool isUsable(CardInstance instance) {
		return instance.Cost <= _currentEnergy;
	}

	/// <summary>
	/// 카드의 효과를 발동한다.
	/// </summary>
	/// <param name="instance">효과 발동할 카드</param>
	/// <param name="context">효과 발동 시의 전투 맥락</param>
	public void UseCard(CardInstance instance, CardUseContext context) {
		SetCurrentEnergy(_currentEnergy - instance.Cost);

		foreach (var action in instance._cardDefinition.actions) {
			action.Execute(context);
		}

		// 과부하: 다음 턴 에너지를 줄이는 상태이상 부여
		if (instance.Keyword.IsOverload) {
			var overload = new Overload();
			overload.Init(context.user, 1, 0);
			context.user.AddStatus(overload);
		}
	}

	public void GainEnergy(int amount) {
		SetCurrentEnergy(_currentEnergy + amount);
	}

	private void SetCurrentEnergy(int energy) {
		_currentEnergy = energy;
		RefreshEnergyText();
		OnEnergyChanged.Invoke(_currentEnergy);
	}

	private void RefreshEnergyText() {
		_energyText.text = $"{_currentEnergy}/{_maxEnergy}";
	}
}
