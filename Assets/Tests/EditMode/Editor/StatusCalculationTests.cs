using NUnit.Framework;
using UnityEngine;

public class StatusCalculationTests {
	private sealed class TestCharacter : CharacterBase {
		public override void PlayIdleAnimation() { }
		public override void PlayAttackAnimation() { }
		public override void PlayHitAnimation() { }
		public override void PlaySkillAnimation() { }
		public override void PlayDeathAnimation() { }
		public override void SetHealth() {
			MaxHealth = 10;
			CurrentHealth = MaxHealth;
		}
	}

	[Test]
	public void HalveDoesNotConsumeStackDuringPreview() {
		GameObject ownerObject = new GameObject("HalveOwner");
		TestCharacter owner = ownerObject.AddComponent<TestCharacter>();
		Halve halve = new Halve();
		halve.Init(owner, 1, 0);

		try {
			Assert.That(halve.PreviewAttackingDamageModifier(10), Is.EqualTo(5));
			Assert.That(halve.TextToShow, Is.EqualTo("1"));

			Assert.That(halve.ApplyAttackingDamageModifier(10), Is.EqualTo(5));
			Assert.That(halve.IsActive, Is.False);
		} finally {
			Object.DestroyImmediate(ownerObject);
		}
	}

	[Test]
	public void HalveDoesNotConsumeStackWhenOwnerTakesDamage() {
		GameObject ownerObject = new GameObject("HalveOwner");
		TestCharacter owner = ownerObject.AddComponent<TestCharacter>();
		Halve halve = new Halve();
		halve.Init(owner, 1, 0);

		try {
			Assert.That(halve.ApplyTakingDamageModifier(10), Is.EqualTo(10));
			Assert.That(halve.TextToShow, Is.EqualTo("1"));
			Assert.That(halve.IsActive, Is.True);
		} finally {
			Object.DestroyImmediate(ownerObject);
		}
	}
}
