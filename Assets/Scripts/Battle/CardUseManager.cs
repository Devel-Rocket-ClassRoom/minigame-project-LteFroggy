using TMPro;
using UnityEngine;

public class CardUseManager : BattleSystemManager {
	private int _maxEnergy = 3;
	private int _currentEnergy;
	private int _nextTurnEnergyPenalty;   // 과부하: 다음 턴 시작 시 깎일 에너지

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;

	public override void StartPlayerTurn() {
		_currentEnergy = Mathf.Max(0, _maxEnergy - _nextTurnEnergyPenalty);
		_nextTurnEnergyPenalty = 0;
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
		_currentEnergy -= instance.Cost;

		foreach (var action in instance._cardDefinition.actions) {
			action.Execute(context);
		}

		// 과부하: 다음 턴 에너지 1 감소 예약
		if (instance.Keyword.IsOverload) {
			_nextTurnEnergyPenalty += 1;
		}
	}

	public void GainEnergy(int amount) {
		_currentEnergy += amount;
	}
	
	private void Update() {
		_energyText.text = $"{_currentEnergy}/{_maxEnergy}";
	}
}
