using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamageNumberView : MonoBehaviour {
	[SerializeField] private TextMeshPro _damageText;
	[SerializeField] private float _lifetime = 0.75f;
	[SerializeField] private float _riseDistance = 0.9f;
	[SerializeField] private Color _startColor = new(1f, 0.23f, 0.08f, 1f);
	[SerializeField] private Color _endColor = new(1f, 0.9f, 0.35f, 0f);

	private Coroutine _playCoroutine;

	private void Awake() {
		if (_damageText == null) _damageText = GetComponent<TextMeshPro>();

		var textRenderer = GetComponent<MeshRenderer>();
		if (textRenderer != null) textRenderer.sortingOrder = 50;
	}

	public void Play(int damage, Vector3 localOffset) {
		if (_damageText == null) _damageText = GetComponent<TextMeshPro>();

		transform.localPosition = localOffset;
		_damageText.text = damage.ToString();
		_damageText.color = _startColor;

		if (_playCoroutine != null) StopCoroutine(_playCoroutine);
		_playCoroutine = StartCoroutine(CoPlay());
	}

	private IEnumerator CoPlay() {
		float elapsed = 0f;
		Vector3 startPosition = transform.localPosition;
		Vector3 endPosition = startPosition + Vector3.up * _riseDistance;

		while (elapsed < _lifetime) {
			float t = elapsed / _lifetime;
			transform.localPosition = Vector3.Lerp(startPosition, endPosition, EaseOut(t));
			_damageText.color = Color.Lerp(_startColor, _endColor, t);

			elapsed += Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}

	private static float EaseOut(float t) {
		return 1f - (1f - t) * (1f - t);
	}
}
