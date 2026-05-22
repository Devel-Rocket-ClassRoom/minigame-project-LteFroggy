public class Greatsword : RelicBase {
	public override string relicId => "0";
	public override RelicRarity rarity => RelicRarity.Common;

	public override int CalculateAmount(CardAction action, CardInstance instance, int amount) {
		// 데미지 주는 액션이고, 태그가 공격일 때만 데미지 2 추가
		if (action is DealDamageCardAction && 
		    instance._cardDefinition.tag == CardTag.Attack) 
		{
			return amount + 2;	
		}
		return amount;
	}
}