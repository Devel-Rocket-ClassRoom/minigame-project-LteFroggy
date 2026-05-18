using UnityEngine;

public class CardUseManager : MonoBehaviour {
	private int _energyMax;
	private int _energyCurrent;
	
	public void StartTurn() {
		_energyCurrent = _energyMax;
	}
	
	public bool UsableCheck(CardInstance instance) {
		return instance.cardDefinition.cost <= _energyCurrent;
	}
	
	public void UseCard(CardInstance instance) {
		if (instance.cardDefinition.cost > _energyCurrent) {
			Debug.Log($"에너지가 충분하지 않습니다.");
			return;
		}
		
		_energyCurrent -= instance.cardDefinition.cost;
		
		foreach (var effect in instance.cardDefinition.effects) {
		}
		
	}
}