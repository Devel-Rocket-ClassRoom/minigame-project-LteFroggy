using System.Text;
using UnityEngine;

public class CardInstance {
	public readonly CardDefinition _cardDefinition;
	public string CardName => StringTableManager.CardNameTable[_cardDefinition.cardName];
	public string TagText => StringTableManager.StringTable[_cardDefinition.tag.ToString()];
	public string RarityText => StringTableManager.StringTable[_cardDefinition.rarity.ToString()];
	public Sprite Icon => _cardDefinition.icon;
	public int Cost => _cardDefinition.cost;
	public bool NeedsTarget => _cardDefinition.needsTarget;
	
	
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
	
	public string GetCardDescriptionWithContext(BattleContext context) {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		foreach (var effect in _cardDefinition.effects) {
			sb.AppendLine(effect.GetCardDescriptionWithContext(context));
		}
		return sb.ToString();
	}
	
	
}