using System.Text;
using UnityEngine;

public class CardInstance : MonoBehaviour {
	public CardDefinition cardDefinition;
	
	private string GetCardTextPreview(BattleContext context) {
		StringBuilder sb = new StringBuilder();
		foreach (var effect in cardDefinition.effects) {
			sb.AppendLine(effect.GetPreviewText(context));
		}
		return sb.ToString();
	}
	
}