public class PlayerCharacter : CharacterBase {
	// MaxHealth, CurrentHealth는 PlayerData에서 받아오기
	public override void SetHealth() {
		MaxHealth = PlayData.Instance.MaxHealth;
		CurrentHealth = PlayData.Instance.CurrentHealth;
	}

	protected override void OnHealthChanged() {
		PlayData.Instance.SetHealth(CurrentHealth);
	}

	// 아직 애니메이션 없음
	public override void PlaySkillAnimation() { }
	public override void PlayAttackAnimation() { }
	public override void PlayHitAnimation() { }
}