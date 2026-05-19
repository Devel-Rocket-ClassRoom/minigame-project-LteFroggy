using System.Text;

public class CardInstance {
	public readonly CardDefinition _cardDefinition;
	
	public CardInstance(CardDefinition cardDefinition) {
		_cardDefinition = cardDefinition;
	}
	
	public string GetCardDescription() {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		foreach (var effect in _cardDefinition.effects) {
			sb.AppendLine(effect.GetCardDescription());
		}
		return sb.ToString();
	}
	
}