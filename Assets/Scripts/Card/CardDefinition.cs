using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Name", menuName = "Card/Card Definition")]
public class CardDefinition : ScriptableObject {
	[Header("=== 기본 카드 정보 ===")]
	public string cardId;
	public CardRarity rarity;
	public Sprite icon;
	public int cost;
	public CardTag tag;
	public bool needsTarget;
	
	public string cardName => $"Card{cardId}Name";
	
	public string StringCardName => StringTableManager.CardNameTable[cardName];
	public string TagText => StringTableManager.StringTable[tag.ToString()];
	public string RarityText => StringTableManager.StringTable[rarity.ToString()];
	[Header("=== 카드 효과들 ===")]
	public List<CardAction> effects;
}