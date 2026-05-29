public class PlayerCharacter : CharacterBase {
	public override void SetHealth() {
		MaxHealth = GamePlayData.Instance.MaxHealth;
		CurrentHealth = GamePlayData.Instance.CurrentHealth;
	}

	protected override void OnHealthChanged() {
		GamePlayData.Instance.SetHealth(CurrentHealth);
	}

	public override void PlayIdleAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Idle");
	}

	public override void PlayAttackAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Attack");
	}

	public override void PlayHitAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Hit");
	}

	public override void PlaySkillAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Skill");
	}

	public override void PlayDeathAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Death");
	}
}
