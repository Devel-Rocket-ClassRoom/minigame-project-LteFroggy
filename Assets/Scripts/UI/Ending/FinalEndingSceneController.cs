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
		new("왕", "문이 열렸군. 왕관이 또 다른 심장을 불러들였나."),
		new("왕", "여기까지 기어오른 반란군은 네가 처음이 아니다."),
		new("플레이어", "왕관은 왕을 선택하지 않는다. 왕을 삼킬 뿐이다."),
		new("왕관", "가까이 오라. 부서진 이름들이 아직 따뜻하다."),
		new("왕", "그렇다면 네 피로 왕관을 잠재워 보아라."),
	};

	private RectTransform _sceneRoot;
	private RectTransform _crownHalo;
	private RectTransform _kingSilhouette;
	private RectTransform _playerSilhouette;
	private RectTransform _slashLine;
	private RectTransform _executionBeam;
	private RectTransform _barrier;
	private Image _kingImage;
	private Image _crownHaloImage;
	private Image _playerImage;
	private Image _slashImage;
	private Image _executionBeamImage;
	private Image _barrierImage;
	private Image _crownGlow;
	private Image _crownCore;
	private Image _stageLight;
	private Image _redFlashOverlay;
	private Image _blackoutOverlay;
	private Image _fadeOverlay;
	private CanvasGroup _kingGroup;
	private CanvasGroup _dialogueGroup;
	private CanvasGroup _summaryGroup;
	private GameObject _dialoguePanel;
	private GameObject _summaryPanel;
	private TextMeshProUGUI _stageText;
	private TextMeshProUGUI _speakerText;
	private TextMeshProUGUI _dialogueText;
	private TextMeshProUGUI _advanceButtonText;
	private TextMeshProUGUI _summaryText;
	private Button _advanceButton;
	private AudioSource _audioSource;
	private AudioClip _crownPulseClip;
	private AudioClip _impactClip;
	private AudioClip _executionClip;

	private int _lineIndex = -1;
	private bool _isSequenceLocked;

	private void Start() {
		EnsureCamera();
		EnsureEventSystem();
		BuildScene();
		PrepareAudio();
		StartCoroutine(CoOpeningSequence());
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
		CreatePanel("Distant Blood Light", _sceneRoot, ColorFromHex(0x3b0b13bb), new Vector2(0.22f, 0.52f), new Vector2(0.78f, 0.98f));
		CreatePanel("Back Wall", _sceneRoot, ColorFromHex(0x240f15ff), new Vector2(0.18f, 0.16f), new Vector2(0.82f, 0.92f));
		CreatePanel("Throne Back", _sceneRoot, ColorFromHex(0x080407ff), new Vector2(0.37f, 0.18f), new Vector2(0.63f, 0.79f));
		CreatePanel("Throne Seat", _sceneRoot, ColorFromHex(0x11070aff), new Vector2(0.32f, 0.16f), new Vector2(0.68f, 0.36f));
		CreatePanel("Throne Step 1", _sceneRoot, ColorFromHex(0x210a0eff), new Vector2(0.24f, 0.1f), new Vector2(0.76f, 0.17f));
		CreatePanel("Throne Step 2", _sceneRoot, ColorFromHex(0x15070aff), new Vector2(0.18f, 0.04f), new Vector2(0.82f, 0.1f));
		CreatePanel("Left Pillar", _sceneRoot, ColorFromHex(0x080306ff), new Vector2(0.08f, 0f), new Vector2(0.16f, 1f));
		CreatePanel("Right Pillar", _sceneRoot, ColorFromHex(0x080306ff), new Vector2(0.84f, 0f), new Vector2(0.92f, 1f));
		CreatePanel("Left Red Slit", _sceneRoot, ColorFromHex(0x8e102033), new Vector2(0.155f, 0.14f), new Vector2(0.166f, 0.9f));
		CreatePanel("Right Red Slit", _sceneRoot, ColorFromHex(0x8e102033), new Vector2(0.834f, 0.14f), new Vector2(0.845f, 0.9f));
		_stageLight = CreatePanel("Stage Light", _sceneRoot, ColorFromHex(0x6f0f1c22), new Vector2(0.21f, 0.02f), new Vector2(0.79f, 0.48f));

		_crownGlow = CreatePanel("Crown Glow", _sceneRoot, ColorFromHex(0x8f102055), new Vector2(0.38f, 0.58f), new Vector2(0.62f, 0.95f));
		_crownHaloImage = CreateAnchoredPanel("Crown Halo", _sceneRoot, ColorFromHex(0xcc1b3033), new Vector2(0f, 335f), new Vector2(460f, 170f));
		_crownHalo = _crownHaloImage.rectTransform;
		_kingImage = CreateAnchoredPanel("King Silhouette", _sceneRoot, ColorFromHex(0x050305ee), new Vector2(0f, 70f), new Vector2(230f, 500f));
		_kingGroup = _kingImage.gameObject.AddComponent<CanvasGroup>();
		_kingSilhouette = _kingImage.rectTransform;
		CreatePanel("King Mantle", _kingSilhouette, ColorFromHex(0x140507ee), new Vector2(-0.22f, 0.1f), new Vector2(1.22f, 0.68f));
		CreatePanel("Crown Left Point", _kingSilhouette, ColorFromHex(0xa01828ee), new Vector2(0.16f, 0.89f), new Vector2(0.32f, 1.06f));
		CreatePanel("Crown Center Point", _kingSilhouette, ColorFromHex(0xe23440ee), new Vector2(0.42f, 0.86f), new Vector2(0.58f, 1.1f));
		CreatePanel("Crown Right Point", _kingSilhouette, ColorFromHex(0xa01828ee), new Vector2(0.68f, 0.89f), new Vector2(0.84f, 1.06f));
		_crownCore = CreatePanel("Crown Core", _kingSilhouette, ColorFromHex(0xff3144ee), new Vector2(0.26f, 0.84f), new Vector2(0.74f, 0.94f));

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

		_stageText = CreateText("Stage Text", _sceneRoot, "옥좌의 끝", 30f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffc3ad00), new Vector2(0.28f, 0.86f), new Vector2(0.72f, 0.94f));

		_dialoguePanel = CreatePanel("Dialogue Panel", _sceneRoot, ColorFromHex(0x09070bea), new Vector2(0.11f, 0.07f), new Vector2(0.89f, 0.29f)).gameObject;
		_dialogueGroup = _dialoguePanel.AddComponent<CanvasGroup>();
		var dialogueRect = (RectTransform)_dialoguePanel.transform;
		CreatePanel("Dialogue Top Line", dialogueRect, ColorFromHex(0xbd3044cc), new Vector2(0f, 0.96f), Vector2.one);
		_speakerText = CreateText("Speaker", dialogueRect, "", 32f, FontStyles.Bold, TextAlignmentOptions.Left, ColorFromHex(0xffd2b8ff), new Vector2(0.04f, 0.61f), new Vector2(0.82f, 0.88f));
		_dialogueText = CreateText("Dialogue", dialogueRect, "", 30f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf7eee6ff), new Vector2(0.04f, 0.18f), new Vector2(0.82f, 0.61f));
		_advanceButton = CreateButton("Advance Button", dialogueRect, "다음", new Vector2(0.83f, 0.18f), new Vector2(0.96f, 0.63f), ShowNextLine);
		_advanceButtonText = _advanceButton.GetComponentInChildren<TextMeshProUGUI>();
		_dialogueGroup.alpha = 0f;

		_summaryPanel = CreatePanel("Run Summary Panel", _sceneRoot, ColorFromHex(0x09070bf2), new Vector2(0.27f, 0.22f), new Vector2(0.73f, 0.76f)).gameObject;
		_summaryGroup = _summaryPanel.AddComponent<CanvasGroup>();
		var summaryRect = (RectTransform)_summaryPanel.transform;
		CreatePanel("Summary Red Line", summaryRect, ColorFromHex(0xbd3044cc), new Vector2(0f, 0.965f), Vector2.one);
		CreateText("Summary Title", summaryRect, "이번 침입은 끝났다", 42f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffb9a6ff), new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.91f));
		_summaryText = CreateText("Summary Text", summaryRect, "", 26f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf4e9ddff), new Vector2(0.1f, 0.33f), new Vector2(0.9f, 0.71f));
		CreateButton("Return Main Button", summaryRect, "메인 메뉴", new Vector2(0.13f, 0.12f), new Vector2(0.45f, 0.26f), ReturnToMainMenu);
		CreateButton("Start Next Run Button", summaryRect, "다음 침입", new Vector2(0.55f, 0.12f), new Vector2(0.87f, 0.26f), StartNextRun);
		_summaryGroup.alpha = 0f;
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
		button.transition = Selectable.Transition.ColorTint;
		button.colors = new ColorBlock {
			normalColor = ColorFromHex(0x3b1018ff),
			highlightedColor = ColorFromHex(0x5f1a28ff),
			pressedColor = ColorFromHex(0x8f2334ff),
			selectedColor = ColorFromHex(0x5f1a28ff),
			disabledColor = ColorFromHex(0x241016aa),
			colorMultiplier = 1f,
			fadeDuration = 0.08f
		};

		CreateText("Text", rect, text, 24f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), Vector2.zero, Vector2.one);
		return button;
	}

	private void PrepareAudio() {
		_audioSource = gameObject.AddComponent<AudioSource>();
		_audioSource.playOnAwake = false;
		_audioSource.spatialBlend = 0f;
		_audioSource.volume = 0.55f;

		_crownPulseClip = CreateToneClip("Crown Pulse", 72f, 0.75f, 0.18f, 0.95f);
		_impactClip = CreateToneClip("Barrier Impact", 128f, 0.28f, 0.32f, 0.65f);
		_executionClip = CreateToneClip("Crown Execution", 44f, 0.95f, 0.42f, 0.9f);
	}

	private IEnumerator CoOpeningSequence() {
		_isSequenceLocked = true;
		_advanceButton.interactable = false;
		_kingImage.color = WithAlpha(_kingImage.color, 0f);
		_crownHaloImage.color = WithAlpha(_crownHaloImage.color, 0f);
		_kingGroup.alpha = 0f;
		_crownGlow.color = WithAlpha(_crownGlow.color, 0f);
		_crownCore.color = WithAlpha(_crownCore.color, 0f);
		_stageLight.color = WithAlpha(_stageLight.color, 0f);
		_stageText.color = WithAlpha(_stageText.color, 0f);
		_crownHalo.localScale = new Vector3(0.76f, 0.76f, 1f);
		_kingSilhouette.localScale = new Vector3(0.92f, 0.92f, 1f);

		yield return FadeImage(_fadeOverlay, 1f, 0f, 1.05f);
		PlayClip(_crownPulseClip, 0.55f);
		yield return FadeText(_stageText, 0f, 1f, 0.45f);
		yield return CoRevealKing();
		yield return FadeCanvasGroup(_dialogueGroup, 0f, 1f, 0.3f);

		_advanceButton.interactable = true;
		_isSequenceLocked = false;
		ShowNextLine();
	}

	private IEnumerator CoRevealKing() {
		const float duration = 1.0f;
		float elapsed = 0f;
		Color king = _kingImage.color;
		Color halo = _crownHaloImage.color;
		Color glow = _crownGlow.color;
		Color core = _crownCore.color;
		Color light = _stageLight.color;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
			king.a = Mathf.Lerp(0f, 0.94f, progress);
			halo.a = Mathf.Lerp(0f, 0.28f, progress);
			glow.a = Mathf.Lerp(0f, 0.42f, progress);
			core.a = Mathf.Lerp(0f, 0.92f, progress);
			light.a = Mathf.Lerp(0f, 0.28f, progress);
			_kingGroup.alpha = progress;
			_kingImage.color = king;
			_crownHaloImage.color = halo;
			_crownGlow.color = glow;
			_crownCore.color = core;
			_stageLight.color = light;
			_kingSilhouette.localScale = Vector3.Lerp(new Vector3(0.92f, 0.92f, 1f), Vector3.one, progress);
			_crownHalo.localScale = Vector3.Lerp(new Vector3(0.76f, 0.76f, 1f), Vector3.one, progress);
			yield return null;
		}
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
		if (line.Speaker == "왕관") {
			PlayClip(_crownPulseClip, 0.65f);
			StartCoroutine(PulseImage(_crownGlow, 0.42f, 0.82f, 0.45f));
		}
	}

	private IEnumerator CoExecutionSequence() {
		_isSequenceLocked = true;
		_advanceButton.interactable = false;
		yield return FadeCanvasGroup(_dialogueGroup, 1f, 0f, 0.18f);
		_dialoguePanel.SetActive(false);

		_stageText.text = "왕관 폭주";
		yield return FadeText(_stageText, _stageText.color.a, 1f, 0.15f);
		yield return CoCrownSurge();
		yield return CoPlayerAttack();
		yield return CoCrownExecution();

		var gamePlayData = FindAnyObjectByType<GamePlayData>();
		if (gamePlayData != null)
			gamePlayData.SetHealth(0);

		_stageText.text = "";
		yield return FadeImage(_blackoutOverlay, 0f, 1f, 0.75f);
		yield return new WaitForSeconds(0.35f);

		_summaryText.text = BuildRunSummary();
		_summaryPanel.transform.SetAsLastSibling();
		_summaryPanel.SetActive(true);
		yield return FadeCanvasGroup(_summaryGroup, 0f, 1f, 0.35f);
	}

	private IEnumerator CoCrownSurge() {
		const float duration = 0.85f;
		float elapsed = 0f;
		PlayClip(_crownPulseClip, 0.75f);

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float progress = elapsed / duration;
			float pulse = Mathf.PingPong(elapsed * 7f, 1f);
			_crownGlow.color = new Color(0.95f, 0f, 0.09f, Mathf.Lerp(0.46f, 0.88f, pulse));
			_crownCore.color = new Color(1f, 0.18f, 0.23f, Mathf.Lerp(0.72f, 1f, pulse));
			_stageLight.color = new Color(0.7f, 0.04f, 0.09f, Mathf.Lerp(0.22f, 0.48f, progress));
			_crownHalo.localScale = Vector3.one * Mathf.Lerp(1f, 1.18f, pulse * progress);
			yield return null;
		}

		yield return ShakeRoot(0.16f, 8f);
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
		PlayClip(_impactClip, 0.85f);
		yield return ShakeRoot(0.22f, 18f);
	}

	private IEnumerator CoCrownExecution() {
		const float duration = 1.15f;
		float elapsed = 0f;
		Vector2 basePlayerPosition = _playerSilhouette.anchoredPosition;
		PlayClip(_executionClip, 1f);

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

	private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration) {
		float elapsed = 0f;
		group.alpha = from;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			group.alpha = Mathf.Lerp(from, to, elapsed / duration);
			yield return null;
		}

		group.alpha = to;
	}

	private IEnumerator FadeText(TextMeshProUGUI text, float from, float to, float duration) {
		float elapsed = 0f;
		Color color = text.color;
		color.a = from;
		text.color = color;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			color.a = Mathf.Lerp(from, to, elapsed / duration);
			text.color = color;
			yield return null;
		}

		color.a = to;
		text.color = color;
	}

	private IEnumerator PulseImage(Image image, float baseAlpha, float peakAlpha, float duration) {
		float elapsed = 0f;
		Color color = image.color;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			float progress = Mathf.Sin((elapsed / duration) * Mathf.PI);
			color.a = Mathf.Lerp(baseAlpha, peakAlpha, progress);
			image.color = color;
			yield return null;
		}

		color.a = baseAlpha;
		image.color = color;
	}

	private void PlayClip(AudioClip clip, float volumeScale) {
		if (_audioSource == null || clip == null) return;
		_audioSource.PlayOneShot(clip, volumeScale);
	}

	private static AudioClip CreateToneClip(string name, float frequency, float duration, float volume, float decay) {
		const int sampleRate = 22050;
		int sampleCount = Mathf.CeilToInt(sampleRate * duration);
		var samples = new float[sampleCount];

		for (int i = 0; i < sampleCount; i++) {
			float t = i / (float)sampleRate;
			float progress = i / (float)sampleCount;
			float envelope = Mathf.Pow(1f - progress, decay);
			float pulse = Mathf.Sin(t * frequency * Mathf.PI) * 0.06f;
			float root = Mathf.Sin(2f * Mathf.PI * (frequency + frequency * pulse) * t);
			float harmonic = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * t) * 0.28f;
			samples[i] = (root + harmonic) * volume * envelope;
		}

		var clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
		clip.SetData(samples, 0);
		return clip;
	}

	private static Color WithAlpha(Color color, float alpha) {
		color.a = alpha;
		return color;
	}

	private string BuildRunSummary() {
		var data = FindAnyObjectByType<GamePlayData>();
		if (data == null)
			return "왕관은 침입자의 기록을 삼켰다.\n다음 침입자는 이미 문 앞에 서 있다.";

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
		var data = FindAnyObjectByType<GamePlayData>();
		if (data != null) data.Reset();
		UISceneBootstrapper.Instance.TransitionTo(k_MainSceneName);
	}

	private void StartNextRun() {
		var data = FindAnyObjectByType<GamePlayData>();
		if (data != null) data.Reset();
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
