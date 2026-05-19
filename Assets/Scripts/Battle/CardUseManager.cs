using System;
using TMPro;
using UnityEngine;

public class CardUseManager : MonoBehaviour {
	private int _energyMax;
	private int _energyCurrent;

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;
	
	public void StartTurn(int energyMax) {
		_energyMax = energyMax;
		_energyCurrent = _energyMax;
	}
	
	public bool UsableCheck(CardInstance instance) {
		return instance._cardDefinition.cost <= _energyCurrent;
	}
	
	public void UseCard(CardInstance instance) {
		if (instance._cardDefinition.cost > _energyCurrent) {
			Debug.Log($"에너지가 충분하지 않습니다.");
			return;
		}
		
		_energyCurrent -= instance._cardDefinition.cost;
		
		foreach (var effect in instance._cardDefinition.effects) {
		}
	}

	private void Update() {
		_energyText.text = $"{_energyCurrent}/{_energyMax}";
	}
}