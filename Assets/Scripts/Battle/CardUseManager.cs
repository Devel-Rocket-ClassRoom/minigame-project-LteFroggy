using TMPro;
using UnityEngine;

public class CardUseManager : BattleSystemManager {
	private int _maxEnergy = 3;
	private int _currentEnergy;

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;
	
	public override void StartPlayerTurn() {
		_currentEnergy = _maxEnergy;
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
		_currentEnergy -= instance._cardDefinition.cost;
		
		foreach (var action in instance._cardDefinition.actions) {
			action.Execute(context);
		}
	}
	
	private void Update() {
		_energyText.text = $"{_currentEnergy}/{_maxEnergy}";
	}
}