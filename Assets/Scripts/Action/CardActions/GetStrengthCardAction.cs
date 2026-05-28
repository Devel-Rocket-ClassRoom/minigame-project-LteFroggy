public class GetStrengthAction : CardAction {
	protected override int Amount { get; }
	protected override int CalculateAmountWithContext(CardUseContext context) {
		throw new System.NotImplementedException();
	}
	public override void Execute(CardUseContext context) {
		throw new System.NotImplementedException();
	}
	public override string CardDescriptionKey { get; }
}