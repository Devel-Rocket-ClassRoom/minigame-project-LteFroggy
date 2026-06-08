using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Get Strength")]
public class GetStrengthCardAction : CardAction {
	public int amount;
	protected override int Amount => amount;
	
	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, amount);
	}
	
	public override void Execute(CardUseContext context) {
		Strength strength = new Strength();
		strength.Init(context.user, ApplyAmountWithContext(context), 0);
		context.user.AddStatus(strength);
		
		context.user.PlaySkillAnimation();
	}
	
	
	
	public override string CardDescriptionKey => "GetStrengthCardText";
}
