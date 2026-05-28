using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Get Strength")]
public class GetStrengthCardAction : CardAction {
	public int amount;
	protected override int Amount => amount;
	
	protected override int CalculateAmountWithContext(CardUseContext context) {
		return amount;
	}
	
	public override void Execute(CardUseContext context) {
		Strength strength = new Strength();
		strength.Init(context.user, amount, 0);
		
		context.user.AddStatus(strength);
	}
	
	
	
	public override string CardDescriptionKey => "GetStrengthCardText";
}