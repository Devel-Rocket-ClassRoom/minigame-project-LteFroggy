public class VulnerableAction : CardAction {
	public int amount;
	protected override int Amount => amount;

	protected override int CalculateAmountWithContext(CardUseContext context) {
		return amount;
	}
	
	public override void Execute(CardUseContext context) {
		Vulnerable vul = new Vulnerable();
		vul.Init(context.target, 0, amount);
		context.target.AddStatus(new Vulnerable());
	}
	
	public override string CardDescriptionKey => "VulnerableCardText";
}