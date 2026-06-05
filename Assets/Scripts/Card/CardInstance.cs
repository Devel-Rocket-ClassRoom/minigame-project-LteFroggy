using System.Text;
using UnityEngine;

public class CardInstance {
	public readonly CardDefinition _cardDefinition;
	public string CardName => StringTableManager.CardNameTable[_cardDefinition.cardName];
	public string TagText => StringTableManager.StringTable[_cardDefinition.tag.ToString()];
	public string RarityText => StringTableManager.StringTable[_cardDefinition.rarity.ToString()];
	public Sprite Icon => _cardDefinition.icon;
	public int Cost => Mathf.Max(0, _cardDefinition.cost + _temporaryCostModifier);
	public bool NeedsTarget => _cardDefinition.needsTarget;
	public CardKeyword Keyword { get; }
	public float BattleAmountMultiplier { get; private set; } = 1f;

	private int _temporaryCostModifier;


	public CardInstance(CardDefinition cardDefinition) {
		_cardDefinition = cardDefinition;
		Keyword = new CardKeyword(cardDefinition.keywords);
	}

	public void AddTemporaryCostModifier(int amount) {
		_temporaryCostModifier += amount;
	}

	public void AddBattleAmountMultiplier(float multiplier) {
		BattleAmountMultiplier *= multiplier;
	}

	public void ResetTurnModifiers() {
		_temporaryCostModifier = 0;
	}

	public void ResetBattleModifiers() {
		_temporaryCostModifier = 0;
		BattleAmountMultiplier = 1f;
		Keyword.Reset(_cardDefinition.keywords);
	}
	
	/// <summary>
	/// BattleContext 없이 순수 카드 설명 가져오기
	/// </summary>
	/// <returns></returns>
	public string GetCardDescription() {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		AppendKeywordLine(sb);
		foreach (var action in _cardDefinition.actions) {
			sb.AppendLine(action.GetCardDescription());
		}
		return sb.ToString();
	}

	public string GetCardDescriptionWithContext(CardUseContext context) {
		StringBuilder sb = new StringBuilder();
		sb.Append($"[{_cardDefinition.TagText}] \n");
		AppendKeywordLine(sb);
		foreach (var action in _cardDefinition.actions) {
			sb.AppendLine(action.GetCardDescriptionWithContext(context));
		}
		return sb.ToString();
	}

	// 태그 줄 아래에 카드가 가진 속성 키워드를 [소모] [선천] 식 한 줄로 추가
	private void AppendKeywordLine(StringBuilder sb) {
		bool hasAny = false;
		foreach (CardKeywordType type in System.Enum.GetValues(typeof(CardKeywordType))) {
			if (type == CardKeywordType.None || !Keyword.Has(type)) continue;
			sb.Append($"[{StringTableManager.StringTable[type.ToString()]}] ");
			hasAny = true;
		}
		if (hasAny) sb.Append('\n');
	}
}
