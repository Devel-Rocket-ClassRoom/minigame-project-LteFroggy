using UnityEngine;

public abstract class ActionBase : ScriptableObject {
	// 특정 효과가 적용된다면, 효과를 얼마나 적용할지
	public abstract int Amount { get; }
	
	// 공격자, 방어자의 Status를 고려해 실제 효과가 적용될 양 계산
	public abstract int CalculateAmountWithContext(BattleContext context);
	
	// 강화되었을 때 사용할 초록색 텍스트
	protected string GetGreenText(string text) => $"<color=#00FF00>{text}</color>";
	// 열화되었을 때 사용할 빨간색 텍스트
	protected string GetRedText(string text) => $"<color=#FF0000>{text}</color>";
	
	public abstract void Execute(BattleContext context);
}