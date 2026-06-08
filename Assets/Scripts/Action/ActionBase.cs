using UnityEngine;

public enum CalculationMode {
	// UI text, tooltip, enemy intent처럼 보여주기만 할 때 사용
	// 이 경로에서는 상태 스택, 지속시간, 1회성 효과를 소모하면 안 됨
	Preview,
	// 카드 실행, 적 행동, 실제 유물 발동처럼 게임 상태에 반영할 때 사용한다.
	// 이 경로에서는 반감 스택처럼 실제 적용 시 소모되는 상태를 처리해야 함
	Apply
}

public abstract class ActionBase<TContext> : ScriptableObject where TContext : BattleContextBase {
	protected abstract int Amount { get; }

	// 액션별 기본 계산식은 하나만 둔다.
	// mode를 통해 상태 modifier를 Preview/Apply 중 어느 쪽으로 사용할지 고른다.
	protected abstract int CalculateAmountWithContext(TContext context, CalculationMode mode);

	// 카드 설명, 적 Intent, hover tooltip 등 미리보기 전용.
	// 호출 결과가 화면 표시용이어야 하며 상태를 바꾸면 안 된다.
	protected int CalculatePreviewAmountWithContext(TContext context) {
		return CalculateAmountWithContext(context, CalculationMode.Preview);
	}

	// 실제 Execute 흐름 전용.
	// 데미지/방어도/화상 수치를 계산하면서 상태 소모가 필요하면 여기서만 일어난다.
	protected int ApplyAmountWithContext(TContext context) {
		return CalculateAmountWithContext(context, CalculationMode.Apply);
	}

	// 아래 helper들은 mode에 따라 CharacterBase의 Preview/Apply modifier를 선택한다.
	// 액션 구현부에서 직접 분기문을 반복하지 않기 위한 공통 진입점이다.
	protected int CalculateAttackingDamageModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyAttackingDamageModifiers(amount)
			: character.PreviewAttackingDamageModifiers(amount);
	}

	protected int CalculateTakingDamageModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyTakingDamageModifiers(amount)
			: character.PreviewTakingDamageModifiers(amount);
	}

	protected int CalculateGainingArmorModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyGainingArmorModifiers(amount)
			: character.PreviewGainingArmorModifiers(amount);
	}

	protected int CalculateGivingBurnModifiers(CharacterBase character, int amount, CalculationMode mode) {
		return mode == CalculationMode.Apply
			? character.ApplyGivingBurnModifiers(amount)
			: character.PreviewGivingBurnModifiers(amount);
	}

	protected string GetGreenText(string text) => $"<color=#00FF00>{text}</color>";
	protected string GetRedText(string text) => $"<color=#FF0000>{text}</color>";

	public abstract void Execute(TContext context);
}
