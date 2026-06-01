using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Hand Defense Count Armor")]
public class HandDefenseCountArmorAction : CardAction {
	public int amount;

	protected override int Amount => amount;
	public override string CardDescriptionKey => "HandDefenseCountArmorCardText";

	public override void Execute(CardUseContext context) {
		int defenseCount = 0;
		foreach (var card in context.battleManager.DeckManager.HandPile) {
			if (card._cardDefinition.tag == CardTag.Defense) defenseCount++;
		}
		int armorAmount = defenseCount * CalculateAmountWithContext(context);
		context.user.AddBlock(context.user.CalculateGainingArmor(armorAmount));
		context.user.PlaySkillAnimation();
	}

	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		return result;
	}
}
