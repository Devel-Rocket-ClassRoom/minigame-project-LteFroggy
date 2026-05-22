public class PlayerCharacter : CharacterBase {
	// MaxHealth, CurrentHealth는 PlayerData에서 받아오기
	public override void SetHealth() {
		MaxHealth = PlayerData.Instance.MaxHealth;
		CurrentHealth = PlayerData.Instance.CurrentHealth;
	}
	
	// 아직 애니메이션 없음
	public override void PlaySkillAnimation() { }
	public override void PlayAttackAnimation() { }
	public override void PlayHitAnimation() { }
}