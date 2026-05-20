using System;
using TMPro;
using UnityEngine;

public class CardUseManager : MonoBehaviour {
	private int _energyMax = 3;
	private int _energyCurrent;

	[Header("=== 에너지 표시 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _energyText;
	
	public void StartTurn() {
		_energyCurrent = _energyMax;
	}
	
	public bool isUsable(CardInstance instance) {
		return instance.Cost <= _energyCurrent;
	}
	
	public void UseCard(CardInstance instance) {
		_energyCurrent -= instance._cardDefinition.cost;
		
		foreach (var effect in instance._cardDefinition.effects) {
			Debug.Log($"{effect.GetCardDescription()} 수행됨!");
		}
	}

	private void Update() {
		_energyText.text = $"{_energyCurrent}/{_energyMax}";
	}
}