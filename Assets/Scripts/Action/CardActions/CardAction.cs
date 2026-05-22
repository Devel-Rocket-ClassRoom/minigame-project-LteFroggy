public abstract class CardAction : ActionBase<CardUseContext> {
	public abstract string CardDescriptionKey { get; }
	
	/// <summary>
	/// 카드 기본 텍스트 반환
	/// </summary>
	public string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey].Replace("@", Amount.ToString());
	}
	
	/// <summary>
	/// 맥락 기반 텍스트 반환. 강화되었으면 초록색, 약화되면 빨간색
	/// </summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public string GetCardDescriptionWithContext(CardUseContext ctx) {
		int calculatedAmount = CalculateAmountWithContext(ctx);
		string replaceText;
		// 강화되었으면 초록색 텍스트
		if (calculatedAmount > Amount) { replaceText = GetGreenText(calculatedAmount.ToString()); }
		// 약화되면, 빨간색 텍스트
		else if (calculatedAmount < Amount) { replaceText = GetRedText(calculatedAmount.ToString()); }
		// 별 일 없으면 그냥 초록색
		else { replaceText = calculatedAmount.ToString(); }
		
		return StringTableManager.StringTable[CardDescriptionKey].Replace("@", replaceText);
	}
}