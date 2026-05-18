using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Name", menuName = "Card/Card Definition")]
public class CardDefinition : ScriptableObject {
	[Header("=== 기본 카드 정보 ===")]
	public string cardId;
	public string cardName;
	public CardRarity rarity;
	
	public Sprite icon;
	
	public int cost;
	public CardTag tag;
	
	public bool needsTarget;

	[Header("=== 카드 효과들 ===")]
	public List<CardEffect> effects;
}