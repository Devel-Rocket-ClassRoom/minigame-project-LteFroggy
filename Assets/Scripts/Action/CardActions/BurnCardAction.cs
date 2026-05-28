using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Give Burn")]
public class BurnCardAction : CardAction {
	public int amount;
	protected override int Amount => amount;
	protected override int CalculateAmountWithContext(CardUseContext context) {
		int result = amount;
		result = context.user.CalculateGiveBurn(result);
		result = context.relicManager.CalculateAmountWithRelics(context.cardInfo, this, result);
		
		return result;
	}
	
	public override void Execute(CardUseContext context) {
		var burn = new Burn();
		burn.Init(context.target, CalculateAmountWithContext(context), 0);
		context.target.AddStatus(burn);
	}
	
	public override string CardDescriptionKey => "BurnCardText";
}