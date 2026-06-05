using System.Collections;
using TMPro;
using UnityEngine;

public class InsufficientEnergyPanel : MonoBehaviour {
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private TextMeshProUGUI _messageText;
	[SerializeField] private string _messageKey = "InsufficientEnergy";
	[SerializeField] private float _fadeInDuration = 0.15f;
	[SerializeField] private float _showDuration = 0.8f;
	[SerializeField] private float _fadeOutDuration = 0.2f;

	private Coroutine _showCoroutine;

	private void Awake() {
		EnsureReferences();
		SetAlpha(0f);
	}

	public void Show() {
		EnsureReferences();
		if (!gameObject.activeSelf) gameObject.SetActive(true);

		if (_showCoroutine != null) StopCoroutine(_showCoroutine);
		_showCoroutine = StartCoroutine(CoShow());
	}

	private void EnsureReferences() {
		if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
		if (_messageText != null && StringTableManager.StringTable.TryGetValue(_messageKey, out string message))
			_messageText.text = message;
	}

	private IEnumerator CoShow() {
		float startAlpha = _canvasGroup != null ? _canvasGroup.alpha : 0f;
		yield return CoFade(startAlpha, 1f, _fadeInDuration);
		yield return new WaitForSeconds(_showDuration);
		yield return CoFade(1f, 0f, _fadeOutDuration);

		_showCoroutine = null;
		gameObject.SetActive(false);
	}

	private IEnumerator CoFade(float startAlpha, float endAlpha, float duration) {
		if (duration <= 0f) {
			SetAlpha(endAlpha);
			yield break;
		}

		float timer = 0f;
		while (timer < duration) {
			timer += Time.deltaTime;
			SetAlpha(Mathf.Lerp(startAlpha, endAlpha, timer / duration));
			yield return null;
		}

		SetAlpha(endAlpha);
	}

	private void SetAlpha(float alpha) {
		if (_canvasGroup != null) _canvasGroup.alpha = alpha;
	}
}
