using TMPro;
using UnityEngine;

public class CardUseManager : BattleSystemManager {
	private int _energyMax = 3;
	private int _energyCurrent;

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;
	
	public override void StartPlayerTurn() {
		_energyCurrent = _energyMax;
	}
	
	/// <summary>
	/// 특정 카드 사용 가능 여부 확인
	/// </summary>
	/// <param name="instance">판별할 카드</param>
	/// <returns>사용 가능 여부</returns>
	public bool isUsable(CardInstance instance) {
		return instance.Cost <= _energyCurrent;
	}

	/// <summary>
	/// 카드의 효과를 발동한다.
	/// </summary>
	/// <param name="instance">효과 발동할 카드</param>
	/// <param name="context">효과 발동 시의 전투 맥락</param>
	public void UseCard(CardInstance instance, BattleContext context) {
		_energyCurrent -= instance._cardDefinition.cost;
		
		foreach (var effect in instance._cardDefinition.effects) {
			effect.Execute(context);
		}
	}
	
	private void Update() {
		_energyText.text = $"{_energyCurrent}/{_energyMax}";
	}
}