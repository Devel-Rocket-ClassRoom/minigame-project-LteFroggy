using UnityEngine;

public abstract class CardAction : ScriptableObject {
	public abstract int Amount { get; }
	public abstract string DescriptionKey { get; }
	
	public abstract void Execute(BattleContext context);
	/// <summary>
	/// 카드 기본 텍스트 반환
	/// </summary>
	public virtual string GetCardDescription() {
		return StringTableManager.StringTable[DescriptionKey].Replace("@", Amount.ToString());
	}
	
	/// <summary>
	/// 맥락 기반 텍스트 반환. 강화되었으면 초록색, 약화되면 빨간색
	/// </summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	public virtual string GetCardDescriptionWithContext(BattleContext ctx) {
		int calculatedAmount = CalculateAmountWithContext(ctx);
		string replaceText;
		// 강화되었으면 초록색 텍스트
		if (calculatedAmount > Amount) { replaceText = GetGreenText(calculatedAmount.ToString()); }
		// 약화되면, 빨간색 텍스트
		else if (calculatedAmount < Amount) { replaceText = GetRedText(calculatedAmount.ToString()); }
		// 별 일 없으면 그냥 초록색
		else { replaceText = calculatedAmount.ToString(); }
		
		return StringTableManager.StringTable[DescriptionKey].Replace("@", replaceText);
	}
	
	public abstract int CalculateAmountWithContext(BattleContext ctx);
	private string GetGreenText(string text) => $"<color=#00FF00>{text}</color>";
	private string GetRedText(string text) => $"<color=#FF0000>{text}</color>";
}