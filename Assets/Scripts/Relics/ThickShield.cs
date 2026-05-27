public class ThickShield : RelicBase {
	public override string relicId => "1";
	public override int cost => 2;
	public override int effectAmount => 3;
	public override CardTag? affectedTag => CardTag.Defense;
	public override RelicRarity rarity => RelicRarity.Common;

	// 방어도 얻는 카드 사용 시 적용됨
	public override int CalculateAmount(CardAction action, CardInstance instance, int amount) {
		if (action is GainArmorCardAction && instance._cardDefinition.tag == CardTag.Defense) {
			return amount + 3;
		}
		return amount;
	}
}