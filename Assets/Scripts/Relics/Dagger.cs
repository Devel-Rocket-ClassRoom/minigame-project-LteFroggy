public class Dagger : RelicBase {
	public override string relicId => "2";
	public override RelicRarity rarity => RelicRarity.Common;

	// 연속 공격 카드인 경우에만 1회 추가
	public override int CalculateRepeat(CardAction action, CardInstance instance, int repeat) {
		if (action is RepeatDealDamageAction && instance._cardDefinition.tag == CardTag.MultiHit) {
			return repeat + 1;
		}
		return repeat;
	}
}