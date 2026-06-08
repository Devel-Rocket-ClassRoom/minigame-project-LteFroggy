public abstract class CardAction : ActionBase<CardUseContext> {
	public abstract string CardDescriptionKey { get; }
	public virtual bool IsDamageAction => false;
	public virtual bool IsBlockAction => false;
	public virtual bool IsHealAction => false;
	public virtual bool IsBurnAction => false;
	
	/// <summary>
	/// 카드 기본 텍스트 반환. 카드 설명에서 사용한다.
	/// </summary>
	public virtual string GetCardDescription() {
		return StringTableManager.StringTable[CardDescriptionKey].Replace("-", Amount.ToString());
	}
	
	/// <summary>
	/// 맥락 기반 텍스트 반환. 강화되었으면 초록색, 약화되면 빨간색
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public virtual string GetCardDescriptionWithContext(CardUseContext context) {
		int calculatedAmount = CalculatePreviewAmountWithContext(context);
		string replaceText = FormatPreviewAmount(calculatedAmount, Amount);
		
		return StringTableManager.StringTable[CardDescriptionKey].Replace("-", replaceText);
	}

	protected string FormatPreviewAmount(int calculatedAmount, int baseAmount) {
		if (calculatedAmount > baseAmount) return GetGreenText(calculatedAmount.ToString());
		if (calculatedAmount < baseAmount) return GetRedText(calculatedAmount.ToString());
		return calculatedAmount.ToString();
	}
}
