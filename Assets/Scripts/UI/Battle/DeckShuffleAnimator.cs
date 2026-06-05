using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeckShuffleAnimator : MonoBehaviour {
	[SerializeField] private RectTransform _animationRoot;
	[SerializeField] private Sprite _cardSprite;
	[SerializeField] private Vector2 _cardSize = new Vector2(48f, 64f);
	[SerializeField] private Color _cardColor = Color.white;
	[SerializeField] private float _moveDuration = 0.55f;
	[SerializeField] private float _maxStartDelay = 0.12f;
	[SerializeField] private float _positionJitter = 18f;
	[SerializeField] private float _curveSideOffset = 130f;
	[SerializeField] private Vector2 _curveHeightRange = new Vector2(70f, 170f);
	[SerializeField] private float _rotationRange = 24f;

	private void Awake() {
		if (_animationRoot != null) return;

		Canvas canvas = GetComponentInParent<Canvas>();
		_animationRoot = canvas != null ? canvas.transform as RectTransform : transform as RectTransform;
	}

	public IEnumerator PlayShuffle(RectTransform from, RectTransform to, int count) {
		if (from == null || to == null || count <= 0 || _animationRoot == null) yield break;

		int runningAnimations = count;
		for (int i = 0; i < count; i++) {
			float delay = Random.Range(0f, _maxStartDelay);
			StartCoroutine(CoMoveShuffleCard(from.position, to.position, delay, () => runningAnimations--));
		}

		while (runningAnimations > 0) {
			yield return null;
		}
	}

	private IEnumerator CoMoveShuffleCard(Vector3 from, Vector3 to, float delay, System.Action onComplete) {
		if (delay > 0f) yield return new WaitForSeconds(delay);

		Image cardImage = CreateCardImage();
		RectTransform cardTransform = cardImage.rectTransform;

		Vector3 start = from + GetRandomJitter();
		Vector3 end = to + GetRandomJitter();
		Vector3 dir = end - start;
		Vector3 side = new Vector3(-dir.y, dir.x, 0f);
		if (side.sqrMagnitude < 0.001f) side = Vector3.right;
		side.Normalize();

		float sideOffset = Random.Range(-_curveSideOffset, _curveSideOffset);
		float heightOffset = Random.Range(_curveHeightRange.x, _curveHeightRange.y);
		Vector3 curveOffset = side * sideOffset + Vector3.up * heightOffset;
		Vector3 p1 = start + dir * 0.25f + curveOffset;
		Vector3 p2 = start + dir * 0.75f - side * sideOffset + Vector3.up * heightOffset;

		float startRotation = Random.Range(-_rotationRange, _rotationRange);
		float endRotation = Random.Range(-_rotationRange, _rotationRange);
		cardTransform.position = start;
		cardTransform.localRotation = Quaternion.Euler(0f, 0f, startRotation);

		float timer = 0f;
		while (timer < _moveDuration) {
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / _moveDuration);
			float easedT = Mathf.SmoothStep(0f, 1f, t);

			cardTransform.position = Bezier.GetBezierPoint(start, p1, p2, end, easedT);
			cardTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startRotation, endRotation, easedT));

			Color color = _cardColor;
			color.a *= Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0.78f, 1f, t));
			cardImage.color = color;

			yield return null;
		}

		Destroy(cardImage.gameObject);
		onComplete?.Invoke();
	}

	private Image CreateCardImage() {
		GameObject cardObject = new GameObject("ShuffleCard");
		cardObject.transform.SetParent(_animationRoot, false);
		cardObject.transform.SetAsLastSibling();

		RectTransform rectTransform = cardObject.AddComponent<RectTransform>();
		rectTransform.sizeDelta = _cardSize;
		rectTransform.localScale = Vector3.one;

		CanvasRenderer canvasRenderer = cardObject.AddComponent<CanvasRenderer>();
		canvasRenderer.cullTransparentMesh = true;

		Image image = cardObject.AddComponent<Image>();
		image.sprite = _cardSprite;
		image.color = _cardColor;
		image.raycastTarget = false;
		image.preserveAspect = true;
		return image;
	}

	private Vector3 GetRandomJitter() {
		return new Vector3(
			Random.Range(-_positionJitter, _positionJitter),
			Random.Range(-_positionJitter, _positionJitter),
			0f
		);
	}
}
