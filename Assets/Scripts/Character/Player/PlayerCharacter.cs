public class PlayerCharacter : CharacterBase {
	// MaxHealth, CurrentHealth는 PlayerData에서 받아오기
	public override void SetHealth() {
		MaxHealth = PlayerData.Instance.MaxHealth;
		CurrentHealth = PlayerData.Instance.CurrentHealth;
	}
}