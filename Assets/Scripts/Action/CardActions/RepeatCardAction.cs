using System.Collections;

public abstract class RepeatCardAction : CardAction {
	public abstract int Repeat { get; }
	
	protected abstract int CalculateRepeatWithContext(CardUseContext context);
	protected abstract IEnumerator ExecuteRepeat(CardUseContext context);
	
	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey].Replace("#", Repeat.ToString()).Replace("-", Amount.ToString());
	}
	
	public override string GetCardDescriptionWithContext(CardUseContext context) {
		int calculatedAmount = CalculatePreviewAmountWithContext(context);
		int calculatedRepeat = CalculateRepeatWithContext(context);
		string replaceAmount = FormatPreviewAmount(calculatedAmount, Amount);
		string replaceRepeat = FormatPreviewAmount(calculatedRepeat, Repeat);
		
		// Amount와 Repeat를 모두 가지게 한다.
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("#", replaceRepeat)
			.Replace("-", replaceAmount);	
	}
}
