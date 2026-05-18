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
		
	}
}