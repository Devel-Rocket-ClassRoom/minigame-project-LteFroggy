using System.Text;
using UnityEngine;

public class CardInstance : MonoBehaviour {
	public CardDefinition cardDefinition;
	
	public string GetCardDescription() {
		StringBuilder sb = new StringBuilder();
		foreach (var effect in cardDefinition.effects) {
			sb.AppendLine(effect.GetPreviewText());
		}
		return sb.ToString();
	}
	
}