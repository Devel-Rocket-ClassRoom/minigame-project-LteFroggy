using System.Collections;

public abstract class RepeatCardAction : CardAction {
	public abstract int Repeat { get; }
	
	protected abstract int CalculateRepeatWithContext(CardUseContext context);
	protected abstract IEnumerator ExecuteRepeat(CardUseContext context);
	
	public override string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey].Replace("@", Amount.ToString()).Replace("#", Repeat.ToString());
	}
	
	public override string GetCardDescriptionWithContext(CardUseContext context) {
		int calculatedAmount = CalculateAmountWithContext(context);
		int calculatedRepeat = CalculateRepeatWithContext(context);
		string replaceAmount;
		string replaceRepeat;

		// Amount 계산
		// 강화되었으면 초록색 텍스트
		if (calculatedAmount > Amount) { replaceAmount = GetGreenText(calculatedAmount.ToString()); }
		// 약화되면, 빨간색 텍스트
		else if (calculatedAmount < Amount) { replaceAmount = GetRedText(calculatedAmount.ToString()); }
		// 별 일 없으면 그냥 초록색
		else { replaceAmount = calculatedAmount.ToString(); }
		
		// 강화되었으면 초록색 텍스트
		if (calculatedRepeat > Repeat) { replaceRepeat = GetGreenText(calculatedRepeat.ToString()); }
		// 약화되면, 빨간색 텍스트
		else if (calculatedRepeat < Repeat) { replaceRepeat = GetRedText(calculatedRepeat.ToString()); }
		// 별 일 없으면 그냥 초록색
		else { replaceRepeat = calculatedRepeat.ToString(); }
		
		// Amount와 Repeat를 모두 가지게 한다.
		return StringTableManager.StringTable[CardDescriptionKey]
			.Replace("@", replaceAmount)
			.Replace("#", replaceRepeat);	
	}
}