using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FinalEndingSceneController : MonoBehaviour {
	private const string k_MainSceneName = "MainScene";
	private const string k_StartSceneName = "StartScene";

	private readonly EndingLine[] _endingLines = {
		new("왕", "여기까지 기어오른 반란군은 네가 처음이 아니다."),
		new("플레이어", "왕관은 왕을 선택하지 않는다. 왕을 삼킬 뿐이다."),
		new("왕", "그렇다면 네 피로 왕관을 잠재워 보아라."),
	};

	private RectTransform _sceneRoot;
	private RectTransform _kingSilhouette;
	private RectTransform _playerSilhouette;
	private RectTransform _slashLine;
	private RectTransform _executionBeam;
	private RectTransform _barrier;
	private Image _playerImage;
	private Image _slashImage;
	private Image _executionBeamImage;
	private Image _barrierImage;
	private Image _crownGlow;
	private Image _redFlashOverlay;
	private Image _blackoutOverlay;
	private Image _fadeOverlay;
	private GameObject _dialoguePanel;
	private GameObject _summaryPanel;
	private TextMeshProUGUI _speakerText;
	private TextMeshProUGUI _dialogueText;
	private TextMeshProUGUI _advanceButtonText;
	private TextMeshProUGUI _summaryText;
	private Button _advanceButton;

	private int _lineIndex = -1;
	private bool _isSequenceLocked;

	private void Start() {
		EnsureCamera();
		EnsureEventSystem();
		BuildScene();
		StartCoroutine(FadeImage(_fadeOverlay, 1f, 0f, 1.2f));
		ShowNextLine();
	}

	private void Update() {
		if (_isSequenceLocked || _summaryPanel.activeSelf) return;

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
			ShowNextLine();
		}
	}

	private void EnsureCamera() {
		if (Camera.main != null) return;

		var cameraObject = new GameObject("Ending Camera", typeof(Camera), typeof(AudioListener));
		cameraObject.tag = "MainCamera";

		var camera = cameraObject.GetComponent<Camera>();
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = new Color(0.015f, 0.008f, 0.012f, 1f);
	}

	private void EnsureEventSystem() {
		if (FindAnyObjectByType<EventSystem>() != null) return;

		new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
	}

	private void BuildScene() {
		var canvasObject = new GameObject("Ending Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		var canvas = canvasObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 200;

		var scaler = canvasObject.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.matchWidthOrHeight = 0.5f;

		_sceneRoot = (RectTransform)canvasObject.transform;

		CreatePanel("Throne Background", _sceneRoot, ColorFromHex(0x14070cff), Vector2.zero, Vector2.one);
		CreatePanel("Back Wall", _sceneRoot, ColorFromHex(0x240f15ff), new Vector2(0.18f, 0.16f), new Vector2(0.82f, 0.92f));
		CreatePanel("Throne", _sceneRoot, ColorFromHex(0x0a0508ff), new Vector2(0.41f, 0.17f), new Vector2(0.59f, 0.72f));
		CreatePanel("Left Pillar", _sceneRoot, ColorFromHex(0x080306ff), new Vector2(0.08f, 0f), new Vector2(0.16f, 1f));
		CreatePanel("Right Pillar", _sceneRoot, ColorFromHex(0x080306ff), new Vector2(0.84f, 0f), new Vector2(0.92f, 1f));

		_crownGlow = CreatePanel("Crown Glow", _sceneRoot, ColorFromHex(0x8f102055), new Vector2(0.38f, 0.58f), new Vector2(0.62f, 0.95f));
		_kingSilhouette = CreateAnchoredPanel("King Silhouette", _sceneRoot, ColorFromHex(0x050305ee), new Vector2(0f, 70f), new Vector2(210f, 470f)).rectTransform;
		CreatePanel("Crown Light", _kingSilhouette, ColorFromHex(0xd32233cc), new Vector2(0.22f, 0.86f), new Vector2(0.78f, 1.02f));

		_playerImage = CreateAnchoredPanel("Player Silhouette", _sceneRoot, ColorFromHex(0xd6d1c7dd), new Vector2(-520f, -230f), new Vector2(80f, 180f));
		_playerSilhouette = _playerImage.rectTransform;

		_slashImage = CreateAnchoredPanel("Player Slash", _sceneRoot, Color.clear, new Vector2(-410f, -155f), new Vector2(430f, 18f));
		_slashLine = _slashImage.rectTransform;
		_slashLine.localRotation = Quaternion.Euler(0f, 0f, 22f);

		_barrierImage = CreateAnchoredPanel("Crown Barrier", _sceneRoot, Color.clear, new Vector2(-25f, 10f), new Vector2(28f, 560f));
		_barrier = _barrierImage.rectTransform;

		_executionBeamImage = CreateAnchoredPanel("Execution Beam", _sceneRoot, Color.clear, new Vector2(-300f, -100f), new Vector2(680f, 32f));
		_executionBeam = _executionBeamImage.rectTransform;
		_executionBeam.localRotation = Quaternion.Euler(0f, 0f, -34f);

		_dialoguePanel = CreatePanel("Dialogue Panel", _sceneRoot, ColorFromHex(0x09070bea), new Vector2(0.11f, 0.07f), new Vector2(0.89f, 0.29f)).gameObject;
		var dialogueRect = (RectTransform)_dialoguePanel.transform;
		_speakerText = CreateText("Speaker", dialogueRect, "", 32f, FontStyles.Bold, TextAlignmentOptions.Left, ColorFromHex(0xffd2b8ff), new Vector2(0.04f, 0.61f), new Vector2(0.82f, 0.88f));
		_dialogueText = CreateText("Dialogue", dialogueRect, "", 30f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf7eee6ff), new Vector2(0.04f, 0.18f), new Vector2(0.82f, 0.61f));
		_advanceButton = CreateButton("Advance Button", dialogueRect, "다음", new Vector2(0.83f, 0.18f), new Vector2(0.96f, 0.63f), ShowNextLine);
		_advanceButtonText = _advanceButton.GetComponentInChildren<TextMeshProUGUI>();

		_summaryPanel = CreatePanel("Run Summary Panel", _sceneRoot, ColorFromHex(0x09070bf2), new Vector2(0.27f, 0.22f), new Vector2(0.73f, 0.76f)).gameObject;
		var summaryRect = (RectTransform)_summaryPanel.transform;
		CreateText("Summary Title", summaryRect, "이번 침입은 끝났다", 42f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffb9a6ff), new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.91f));
		_summaryText = CreateText("Summary Text", summaryRect, "", 26f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf4e9ddff), new Vector2(0.1f, 0.33f), new Vector2(0.9f, 0.71f));
		CreateButton("Return Main Button", summaryRect, "메인 메뉴", new Vector2(0.13f, 0.12f), new Vector2(0.45f, 0.26f), ReturnToMainMenu);
		CreateButton("Start Next Run Button", summaryRect, "다음 침입", new Vector2(0.55f, 0.12f), new Vector2(0.87f, 0.26f), StartNextRun);
		_summaryPanel.SetActive(false);

		_redFlashOverlay = CreatePanel("Red Flash Overlay", _sceneRoot, Color.clear, Vector2.zero, Vector2.one);
		_blackoutOverlay = CreatePanel("Blackout Overlay", _sceneRoot, Color.clear, Vector2.zero, Vector2.one);
		_fadeOverlay = CreatePanel("Fade Overlay", _sceneRoot, Color.black, Vector2.zero, Vector2.one);
	}

	private Image CreatePanel(string objectName, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		var panelObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		panelObject.transform.SetParent(parent, false);

		var rect = (RectTransform)panelObject.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var image = panelObject.GetComponent<Image>();
		image.color = color;
		image.raycastTarget = false;
		return image;
	}

	private Image CreateAnchoredPanel(string objectName, Transform parent, Color color, Vector2 anchoredPosition, Vector2 size) {
		var image = CreatePanel(objectName, parent, color, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
		var rect = image.rectTransform;
		rect.anchoredPosition = anchoredPosition;
		rect.sizeDelta = size;
		return image;
	}

	private TextMeshProUGUI CreateText(string objectName, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		var textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		textObject.transform.SetParent(parent, false);

		var rect = (RectTransform)textObject.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var label = textObject.GetComponent<TextMeshProUGUI>();
		label.text = text;
		label.font = TMP_Settings.defaultFontAsset;
		label.fontSize = size;
		label.fontStyle = style;
		label.alignment = alignment;
		label.color = color;
		label.textWrappingMode = TextWrappingModes.Normal;
		label.raycastTarget = false;
		return label;
	}

	private Button CreateButton(string objectName, Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick) {
		var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		buttonObject.transform.SetParent(parent, false);

		var rect = (RectTransform)buttonObject.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var image = buttonObject.GetComponent<Image>();
		image.color = ColorFromHex(0x3b1018ff);

		var button = buttonObject.GetComponent<Button>();
		button.onClick.AddListener(onClick);

		CreateText("Text", rect, text, 24f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), Vector2.zero, Vector2.one);
		return button;
	}

	private void ShowNextLine() {
		if (_isSequenceLocked) return;

		_lineIndex++;
		if (_lineIndex >= _endingLines.Length) {
			StartCoroutine(CoExecutionSequence());
			return;
		}

		EndingLine line = _endingLines[_lineIndex];
		_speakerText.text = line.Speaker;
		_dialogueText.text = line.Text;
		_advanceButtonText.text = _lineIndex == _endingLines.Length - 1 ? "공격" : "다음";
	}

	private IEnumerator CoExecutionSequence() {
		_isSequenceLocked = true;
		_advanceButton.interactable = false;
		_dialoguePanel.SetActive(false);

		yield return CoPlayerAttack();
		yield return CoCrownExecution();

		GamePlayData.Instance.SetHealth(0);
		yield return FadeImage(_blackoutOverlay, 0f, 1f, 0.75f);
		yield return new WaitForSeconds(0.35f);

		_summaryText.text = BuildRunSummary();
		_summaryPanel.transform.SetAsLastSibling();
		_summaryPanel.SetActive(true);
	}

	private IEnumerator CoPlayerAttack() {
		const float duration = 0.7f;
		float elapsed = 0f;
		Vector2 startPosition = _playerSilhouette.anchoredPosition;
		Vector2 targetPosition = new Vector2(-150f, -145f);

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
			_playerSilhouette.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);
			_slashImage.color = new Color(1f, 0.92f, 0.78f, Mathf.Lerp(0f, 0.95f, progress));
			yield return null;
		}

		_barrierImage.color = ColorFromHex(0xff2338dd);
		yield return ShakeRoot(0.22f, 18f);
	}

	private IEnumerator CoCrownExecution() {
		const float duration = 1.15f;
		float elapsed = 0f;
		Vector2 basePlayerPosition = _playerSilhouette.anchoredPosition;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float progress = elapsed / duration;
			float pulse = Mathf.PingPong(elapsed * 6f, 1f);

			_crownGlow.color = new Color(0.95f, 0f, 0.08f, Mathf.Lerp(0.35f, 0.9f, pulse));
			_executionBeamImage.color = new Color(1f, 0.02f, 0.06f, Mathf.Lerp(0.1f, 0.9f, progress));
			_redFlashOverlay.color = new Color(0.85f, 0f, 0.04f, Mathf.Lerp(0.65f, 0f, progress));
			_playerImage.color = new Color(0.95f, 0.24f, 0.18f, Mathf.Lerp(1f, 0f, progress));
			_slashImage.color = new Color(1f, 0.92f, 0.78f, Mathf.Lerp(0.75f, 0f, progress));
			_barrierImage.color = new Color(1f, 0.12f, 0.2f, Mathf.Lerp(0.85f, 0f, progress));
			_playerSilhouette.anchoredPosition = basePlayerPosition + Random.insideUnitCircle * Mathf.Lerp(24f, 0f, progress);

			yield return null;
		}

		_playerImage.color = Color.clear;
		_slashImage.color = Color.clear;
		_barrierImage.color = Color.clear;
		_redFlashOverlay.color = Color.clear;
	}

	private IEnumerator ShakeRoot(float duration, float strength) {
		float elapsed = 0f;
		Vector2 basePosition = _sceneRoot.anchoredPosition;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			_sceneRoot.anchoredPosition = basePosition + Random.insideUnitCircle * strength;
			yield return null;
		}

		_sceneRoot.anchoredPosition = basePosition;
	}

	private IEnumerator FadeImage(Image image, float from, float to, float duration) {
		float elapsed = 0f;
		Color color = image.color;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			color.a = Mathf.Lerp(from, to, elapsed / duration);
			image.color = color;
			yield return null;
		}

		color.a = to;
		image.color = color;
	}

	private string BuildRunSummary() {
		var data = GamePlayData.Instance;
		var builder = new StringBuilder();
		builder.AppendLine($"최종 덱 카드 수: {data.Deck.Count}");
		builder.AppendLine($"획득 유물 수: {data.Relics.Count}");
		builder.AppendLine($"남은 HP: {data.CurrentHealth}/{data.MaxHealth}");
		builder.Append("주요 유물: ");

		if (data.Relics.Count == 0) {
			builder.Append("-");
			return builder.ToString();
		}

		for (int i = 0; i < data.Relics.Count; i++) {
			if (i > 0) builder.Append(", ");
			builder.Append(GetRelicDisplayName(data.Relics[i]));
		}

		return builder.ToString();
	}

	private static string GetRelicDisplayName(RelicBase relic) {
		try {
			return relic.displayName;
		} catch {
			return relic.GetType().Name;
		}
	}

	private void ReturnToMainMenu() {
		GamePlayData.Instance.Reset();
		UISceneBootstrapper.Instance.TransitionTo(k_MainSceneName);
	}

	private void StartNextRun() {
		GamePlayData.Instance.Reset();
		UISceneBootstrapper.Instance.TransitionTo(k_StartSceneName);
	}

	private static Color ColorFromHex(uint rgba) {
		float r = ((rgba >> 24) & 0xff) / 255f;
		float g = ((rgba >> 16) & 0xff) / 255f;
		float b = ((rgba >> 8) & 0xff) / 255f;
		float a = (rgba & 0xff) / 255f;
		return new Color(r, g, b, a);
	}

	private readonly struct EndingLine {
		public readonly string Speaker;
		public readonly string Text;

		public EndingLine(string speaker, string text) {
			Speaker = speaker;
			Text = text;
		}
	}
}
