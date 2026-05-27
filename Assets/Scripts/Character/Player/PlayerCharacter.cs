public class PlayerCharacter : CharacterBase {
	// MaxHealth, CurrentHealth는 PlayerData에서 받아오기
	public override void SetHealth() {
		MaxHealth = GamePlayData.Instance.MaxHealth;
		CurrentHealth = GamePlayData.Instance.CurrentHealth;
	}

	protected override void OnHealthChanged() {
		GamePlayData.Instance.SetHealth(CurrentHealth);
	}

	// 아직 애니메이션 없음
	public override void PlaySkillAnimation() { }
	public override void PlayAttackAnimation() { }
	public override void PlayHitAnimation() { }
}