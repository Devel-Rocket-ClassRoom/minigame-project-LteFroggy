using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Hand Defense Count Armor")]
public class HandDefenseCountArmorAction : CardAction {
	public int amount;
	public override bool IsBlockAction => true;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "HandDefenseCountArmorCardText";

	public override void Execute(CardUseContext context) {
		int defenseCount = 0;
		foreach (var card in context.battleManager.DeckManager.HandPile) {
			if (card._cardDefinition.tag == CardTag.Defense) defenseCount++;
		}
		int armorAmount = defenseCount * ApplyAmountWithContext(context);
		context.user.AddBlock(CalculateGainingArmorModifiers(context.user, armorAmount, CalculationMode.Apply));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		int result = amount;
		result = context.relicManager.CalculateAmountWithRelics(context, this, result);
		return result;
	}
}
