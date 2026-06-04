using UnityEngine;

public class HitFeedbackView : MonoBehaviour {
	[SerializeField] private Transform _feedbackAnchor;
	[SerializeField] private ParticleSystem _hitParticlePrefab;
	[SerializeField] private DamageNumberView _damageNumberPrefab;
	[SerializeField] private Vector3 _fallbackAnchorOffset = new(0f, 2.2f, 0f);
	[SerializeField] private float _stackResetDelay = 0.45f;
	[SerializeField] private float _stackSpacingX = 0.32f;
	[SerializeField] private float _stackSpacingY = 0.16f;

	private int _stackIndex;
	private float _lastFeedbackTime;

	public void Play(int actualDamage) {
		if (actualDamage <= 0) return;

		Transform anchor = _feedbackAnchor != null ? _feedbackAnchor : transform;
		Vector3 spawnPosition = _feedbackAnchor != null ? anchor.position : transform.position + _fallbackAnchorOffset;

		if (Time.time - _lastFeedbackTime > _stackResetDelay) _stackIndex = 0;
		_lastFeedbackTime = Time.time;

		PlayParticle(spawnPosition);
		PlayDamageNumber(anchor, actualDamage);
	}

	private void PlayParticle(Vector3 spawnPosition) {
		ParticleSystem particle = _hitParticlePrefab == null
			? CreateRuntimeHitParticle(spawnPosition)
			: Instantiate(_hitParticlePrefab, spawnPosition, Quaternion.identity);
		particle.Play();

		var main = particle.main;
		float destroyDelay = main.duration + main.startLifetime.constantMax + 0.2f;
		Destroy(particle.gameObject, destroyDelay);
	}

	private void PlayDamageNumber(Transform anchor, int actualDamage) {
		DamageNumberView damageNumber = _damageNumberPrefab == null
			? CreateRuntimeDamageNumber(anchor)
			: Instantiate(_damageNumberPrefab, anchor);
		damageNumber.Play(actualDamage, GetStackOffset(_stackIndex));
		_stackIndex++;
	}

	private Vector3 GetStackOffset(int index) {
		int lane = index % 5;
		int row = index / 5;
		float x = (lane - 2) * _stackSpacingX;
		float y = row * _stackSpacingY;

		return new Vector3(x, y, 0f);
	}

	private static ParticleSystem CreateRuntimeHitParticle(Vector3 spawnPosition) {
		GameObject particleObject = new("Hit Particle");
		particleObject.transform.position = spawnPosition;

		ParticleSystem particleSystem = particleObject.AddComponent<ParticleSystem>();
		ConfigureRuntimeHitParticle(particleSystem);

		return particleSystem;
	}

	private static void ConfigureRuntimeHitParticle(ParticleSystem particleSystem) {
		var main = particleSystem.main;
		main.duration = 0.18f;
		main.loop = false;
		main.startLifetime = new ParticleSystem.MinMaxCurve(0.24f, 0.42f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(1.1f, 2.4f);
		main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
		main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.28318f);
		main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.2f, 0.05f, 1f), new Color(1f, 0.85f, 0.2f, 0.95f));
		main.gravityModifier = 0f;
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.maxParticles = 32;

		var emission = particleSystem.emission;
		emission.enabled = true;
		emission.rateOverTime = 0f;
		emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 18) });

		var shape = particleSystem.shape;
		shape.enabled = true;
		shape.shapeType = ParticleSystemShapeType.Circle;
		shape.radius = 0.28f;
		shape.arc = 360f;

		var velocity = particleSystem.velocityOverLifetime;
		velocity.enabled = true;
		velocity.space = ParticleSystemSimulationSpace.Local;
		velocity.x = new ParticleSystem.MinMaxCurve(-0.55f, 0.55f);
		velocity.y = new ParticleSystem.MinMaxCurve(0.25f, 1.05f);
		velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

		var color = particleSystem.colorOverLifetime;
		color.enabled = true;
		Gradient gradient = new();
		gradient.SetKeys(
			new[] {
				new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
				new GradientColorKey(new Color(1f, 0.18f, 0.05f), 0.55f),
				new GradientColorKey(new Color(0.45f, 0.04f, 0.02f), 1f)
			},
			new[] {
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0.75f, 0.55f),
				new GradientAlphaKey(0f, 1f)
			}
		);
		color.color = gradient;

		var size = particleSystem.sizeOverLifetime;
		size.enabled = true;
		size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

		ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
		renderer.sortingOrder = 45;
	}

	private static DamageNumberView CreateRuntimeDamageNumber(Transform anchor) {
		GameObject numberObject = new("Damage Number");
		numberObject.transform.SetParent(anchor, false);

		var text = numberObject.AddComponent<TMPro.TextMeshPro>();
		text.fontSize = 4.5f;
		text.alignment = TMPro.TextAlignmentOptions.Center;
		text.color = new Color(1f, 0.23f, 0.08f, 1f);
		text.enableWordWrapping = false;
		text.richText = false;

		MeshRenderer renderer = numberObject.GetComponent<MeshRenderer>();
		if (renderer != null) renderer.sortingOrder = 50;

		return numberObject.AddComponent<DamageNumberView>();
	}
}
