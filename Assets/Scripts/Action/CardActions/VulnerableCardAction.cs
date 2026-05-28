using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Give Vulnerable")]
public class VulnerableCardAction : CardAction {
	public int amount;
	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return amount;
	}
	
	public override void Execute(CardUseContext context) {
		Vulnerable vulnerable = new Vulnerable();
		vulnerable.Init(context.target, 0, amount);
		context.target.AddStatus(vulnerable);
	}
	
	public override string CardDescriptionKey => "VulnerableCardText";
}