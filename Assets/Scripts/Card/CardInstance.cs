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
	
	/// <summary>
	/// BattleContext 없이 순수 카드 설명 가져오기
	/// </summary>
	/// <returns></returns>
	public string GetCardDescription() {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		foreach (var action in _cardDefinition.actions) {
			sb.AppendLine(action.GetCardDescription());
		}
		return sb.ToString();
	}
	
	public string GetCardDescriptionWithContext(CardUseContext context) {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		foreach (var action in _cardDefinition.actions) {
			sb.AppendLine(action.GetCardDescriptionWithContext(context));
		}
		return sb.ToString();
	}
}