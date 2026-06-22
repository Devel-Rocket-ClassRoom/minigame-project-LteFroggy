using UnityEngine;
using UnityEngine.Pool;

public class HitFeedbackView : MonoBehaviour {
	private const string DefaultFeedbackSortingLayer = "Characters";
	private const int DefaultParticleSortingOrder = 45;
	private const int DefaultDamageNumberSortingOrder = 50;

	[SerializeField] private Transform _feedbackAnchor;
	[SerializeField] private ParticleSystem _hitParticlePrefab;
	[SerializeField] private DamageNumberView _damageNumberPrefab;
	[SerializeField] private Vector3 _fallbackAnchorOffset = new(0f, 2.2f, 0f);
	[SerializeField] private float _stackResetDelay = 0.45f;
	[SerializeField] private float _stackSpacingX = 0.32f;
	[SerializeField] private float _stackSpacingY = 0.16f;
	[SerializeField] private string _feedbackSortingLayerName = DefaultFeedbackSortingLayer;
	[SerializeField] private int _hitParticleSortingOrder = DefaultParticleSortingOrder;
	[SerializeField] private int _damageNumberSortingOrder = DefaultDamageNumberSortingOrder;
	[SerializeField] private int _defaultPoolCapacity = 4;
	[SerializeField] private int _maxPoolSize = 24;

	private int _stackIndex;
	private float _lastFeedbackTime;
	private ObjectPool<ParticleSystem> _hitParticlePool;
	private ObjectPool<DamageNumberView> _damageNumberPool;

	private void Awake() {
		_hitParticlePool = new ObjectPool<ParticleSystem>(
			CreateHitParticle,
			OnGetHitParticle,
			OnReleaseHitParticle,
			particle => Destroy(particle.gameObject),
			true,
			_defaultPoolCapacity,
			_maxPoolSize
		);
		_damageNumberPool = new ObjectPool<DamageNumberView>(
			CreateDamageNumber,
			OnGetDamageNumber,
			OnReleaseDamageNumber,
			damageNumber => Destroy(damageNumber.gameObject),
			true,
			_defaultPoolCapacity,
			_maxPoolSize
		);
	}

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
		ParticleSystem particle = _hitParticlePool.Get();
		particle.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
		ConfigureParticleRenderer(particle);
		particle.Clear(true);
		particle.Play();

		var main = particle.main;
		float releaseDelay = main.duration + main.startLifetime.constantMax + 0.2f;
		StartCoroutine(ReleaseParticleAfterDelay(particle, releaseDelay));
	}

	private void PlayDamageNumber(Transform anchor, int actualDamage) {
		DamageNumberView damageNumber = _damageNumberPool.Get();
		damageNumber.transform.SetParent(anchor, false);
		damageNumber.SetSorting(_feedbackSortingLayerName, _damageNumberSortingOrder);
		damageNumber.Play(actualDamage, GetStackOffset(_stackIndex), ReleaseDamageNumber);
		_stackIndex++;
	}

	private Vector3 GetStackOffset(int index) {
		int lane = index % 5;
		int row = index / 5;
		float x = (lane - 2) * _stackSpacingX;
		float y = row * _stackSpacingY;

		return new Vector3(x, y, 0f);
	}

	private ParticleSystem CreateHitParticle() {
		ParticleSystem particle = _hitParticlePrefab == null
			? CreateRuntimeHitParticle()
			: Instantiate(_hitParticlePrefab);
		ConfigureParticleRenderer(particle);
		particle.gameObject.SetActive(false);
		return particle;
	}

	private DamageNumberView CreateDamageNumber() {
		DamageNumberView damageNumber = _damageNumberPrefab == null
			? CreateRuntimeDamageNumber()
			: Instantiate(_damageNumberPrefab);
		damageNumber.SetSorting(_feedbackSortingLayerName, _damageNumberSortingOrder);
		damageNumber.gameObject.SetActive(false);
		return damageNumber;
	}

	private void OnGetHitParticle(ParticleSystem particle) {
		particle.transform.SetParent(null);
		particle.transform.localScale = Vector3.one;
		particle.gameObject.SetActive(true);
	}

	private static void OnReleaseHitParticle(ParticleSystem particle) {
		particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particle.transform.SetParent(null);
		particle.gameObject.SetActive(false);
	}

	private void OnGetDamageNumber(DamageNumberView damageNumber) {
		damageNumber.transform.localScale = Vector3.one;
		damageNumber.gameObject.SetActive(true);
	}

	private static void OnReleaseDamageNumber(DamageNumberView damageNumber) {
		damageNumber.ResetForPool();
		damageNumber.transform.SetParent(null);
		damageNumber.gameObject.SetActive(false);
	}

	private void ReleaseDamageNumber(DamageNumberView damageNumber) {
		_damageNumberPool.Release(damageNumber);
	}

	private System.Collections.IEnumerator ReleaseParticleAfterDelay(ParticleSystem particle, float delay) {
		yield return new WaitForSeconds(delay);
		_hitParticlePool.Release(particle);
	}

	private void ConfigureParticleRenderer(ParticleSystem particle) {
		ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
		if (renderer == null) return;

		renderer.sortingLayerName = _feedbackSortingLayerName;
		renderer.sortingOrder = _hitParticleSortingOrder;
	}

	private ParticleSystem CreateRuntimeHitParticle() {
		GameObject particleObject = new("Hit Particle");

		ParticleSystem particleSystem = particleObject.AddComponent<ParticleSystem>();
		ConfigureRuntimeHitParticle(particleSystem);

		return particleSystem;
	}

	private void ConfigureRuntimeHitParticle(ParticleSystem particleSystem) {
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

		ConfigureParticleRenderer(particleSystem);
	}

	private DamageNumberView CreateRuntimeDamageNumber() {
		GameObject numberObject = new("Damage Number");

		var text = numberObject.AddComponent<TMPro.TextMeshPro>();
		text.fontSize = 4.5f;
		text.alignment = TMPro.TextAlignmentOptions.Center;
		text.color = new Color(1f, 0.23f, 0.08f, 1f);
		text.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
		text.richText = false;

		DamageNumberView damageNumber = numberObject.AddComponent<DamageNumberView>();
		damageNumber.SetSorting(_feedbackSortingLayerName, _damageNumberSortingOrder);
		return damageNumber;
	}
}
