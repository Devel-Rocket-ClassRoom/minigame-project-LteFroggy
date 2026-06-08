using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PauseMenuController : MonoBehaviour {
	private const string MainSceneName = "MainScene";
	private const string PauseTooltipTitle = "일시정지";
	private const string PauseTooltipDescription = "현재 런을 멈추고 계속하거나 메인 메뉴로 돌아갑니다.";

	private Button _pauseButton;
	private Button _resumeButton;
	private Button _mainMenuButton;
	private GameObject _panelRoot;
	private OverlayPanelController _mapOverlay;
	private TMP_FontAsset _fontAsset;
	private bool _initialized;
	private bool _isPaused;

	public void Initialize(Button mapButton, Button deckButton, OverlayPanelController mapOverlay, Sprite pauseIcon, TMP_FontAsset fontAsset) {
		_mapOverlay = mapOverlay;
		_fontAsset = fontAsset;

		EnsurePauseButton(mapButton, deckButton, pauseIcon);
		EnsurePausePanel();

		_initialized = _pauseButton != null && _panelRoot != null;
	}

	private void Update() {
		if (!_initialized) return;
		if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
	}

	private void OnDestroy() {
		if (_pauseButton != null)
			_pauseButton.onClick.RemoveListener(TogglePause);
		if (_resumeButton != null)
			_resumeButton.onClick.RemoveListener(ClosePause);
		if (_mainMenuButton != null)
			_mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);

		if (_isPaused)
			Time.timeScale = 1f;
	}

	public void TogglePause() {
		if (_isPaused) ClosePause();
		else OpenPause();
	}

	public void OpenPause() {
		if (_isPaused || _panelRoot == null) return;

		DescriptionSystem.Hide();
		CardListOverlayController.Instance?.Close();
		_mapOverlay?.Close();

		_isPaused = true;
		Time.timeScale = 0f;
		_panelRoot.SetActive(true);
		_panelRoot.transform.SetAsLastSibling();
	}

	public void ClosePause() {
		if (!_isPaused) return;

		_isPaused = false;
		Time.timeScale = 1f;
		if (_panelRoot != null) _panelRoot.SetActive(false);
		DescriptionSystem.Hide();
	}

	private void ReturnToMainMenu() {
		_isPaused = false;
		Time.timeScale = 1f;
		if (_panelRoot != null) _panelRoot.SetActive(false);

		DescriptionSystem.Hide();
		GamePlayData.Instance.Reset();
		UISceneBootstrapper.Instance.TransitionTo(MainSceneName);
	}

	private void EnsurePauseButton(Button mapButton, Button deckButton, Sprite pauseIcon) {
		if (mapButton == null) return;

		Transform parent = mapButton.transform.parent;
		Transform existing = parent != null ? parent.Find("PauseButton") : null;
		if (existing != null)
			_pauseButton = existing.GetComponent<Button>();

		if (_pauseButton == null) {
			GameObject buttonObject = Instantiate(mapButton.gameObject, parent);
			buttonObject.name = "PauseButton";
			if (deckButton != null)
				buttonObject.transform.SetSiblingIndex(deckButton.transform.GetSiblingIndex() + 1);
			else
				buttonObject.transform.SetSiblingIndex(mapButton.transform.GetSiblingIndex() + 1);

			_pauseButton = buttonObject.GetComponent<Button>();
		}

		foreach (Transform child in _pauseButton.transform)
			child.gameObject.SetActive(false);

		var image = _pauseButton.GetComponent<Image>();
		if (image != null) {
			image.sprite = pauseIcon != null ? pauseIcon : CreateFallbackPauseSprite();
			image.preserveAspect = true;
			image.raycastTarget = true;
			_pauseButton.targetGraphic = image;
		}

		_pauseButton.onClick.RemoveAllListeners();
		_pauseButton.onClick.AddListener(TogglePause);

		var trigger = _pauseButton.GetComponent<DescriptionTooltipTrigger>();
		if (trigger == null) trigger = _pauseButton.gameObject.AddComponent<DescriptionTooltipTrigger>();
		trigger.SetContent(PauseTooltipTitle, PauseTooltipDescription);
	}

	private void EnsurePausePanel() {
		if (_panelRoot != null) return;

		var canvas = GetComponentInParent<Canvas>();
		if (canvas == null) return;

		var rootImage = CreatePanel("PauseOverlayPanel", canvas.transform, ColorFromHex(0x000000b8), Vector2.zero, Vector2.one);
		_panelRoot = rootImage.gameObject;
		rootImage.raycastTarget = true;

		var panelImage = CreatePanel("PausePanel", _panelRoot.transform, ColorFromHex(0x09070bf2), new Vector2(0.36f, 0.32f), new Vector2(0.64f, 0.68f));
		var panelRect = panelImage.rectTransform;

		var outline = panelImage.gameObject.AddComponent<Outline>();
		outline.effectColor = ColorFromHex(0xb88a3eff);
		outline.effectDistance = new Vector2(2f, -2f);

		CreateText("Title", panelRect, "일시정지", 42f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), new Vector2(0.08f, 0.68f), new Vector2(0.92f, 0.9f));
		CreateText("Description", panelRect, "현재 런을 멈췄습니다.", 24f, FontStyles.Normal, TextAlignmentOptions.Center, ColorFromHex(0xf4e9ddff), new Vector2(0.08f, 0.5f), new Vector2(0.92f, 0.68f));

		_resumeButton = CreateButton("ResumeButton", panelRect, "계속하기", new Vector2(0.16f, 0.27f), new Vector2(0.84f, 0.43f));
		_mainMenuButton = CreateButton("MainMenuButton", panelRect, "메인 메뉴로", new Vector2(0.16f, 0.1f), new Vector2(0.84f, 0.26f));

		_resumeButton.onClick.AddListener(ClosePause);
		_mainMenuButton.onClick.AddListener(ReturnToMainMenu);

		_panelRoot.SetActive(false);
	}

	private Image CreatePanel(string objectName, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		go.transform.SetParent(parent, false);

		var rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var image = go.GetComponent<Image>();
		image.color = color;
		image.raycastTarget = false;
		return image;
	}

	private TextMeshProUGUI CreateText(string objectName, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		go.transform.SetParent(parent, false);

		var rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var label = go.GetComponent<TextMeshProUGUI>();
		label.text = text;
		if (_fontAsset != null) label.font = _fontAsset;
		label.fontSize = size;
		label.fontStyle = style;
		label.alignment = alignment;
		label.color = color;
		label.textWrappingMode = TextWrappingModes.Normal;
		label.raycastTarget = false;
		return label;
	}

	private Button CreateButton(string objectName, Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax) {
		var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		go.transform.SetParent(parent, false);

		var rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var image = go.GetComponent<Image>();
		image.color = ColorFromHex(0x3b1018ff);

		var button = go.GetComponent<Button>();
		button.targetGraphic = image;

		CreateText("Text", rect, text, 26f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), Vector2.zero, Vector2.one);
		return button;
	}

	private static Sprite CreateFallbackPauseSprite() {
		const int size = 64;
		var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
		var clear = new Color32(0, 0, 0, 0);
		var pixels = new Color32[size * size];
		for (int i = 0; i < pixels.Length; i++)
			pixels[i] = clear;

		texture.SetPixels32(pixels);
		DrawRect(texture, 10, 10, 44, 44, new Color32(38, 24, 16, 255));
		DrawRect(texture, 14, 14, 36, 36, new Color32(217, 174, 92, 255));
		DrawRect(texture, 24, 20, 6, 24, new Color32(68, 38, 28, 255));
		DrawRect(texture, 34, 20, 6, 24, new Color32(68, 38, 28, 255));
		texture.Apply();
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;

		return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
	}

	private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color32 color) {
		for (int yy = y; yy < y + height; yy++) {
			for (int xx = x; xx < x + width; xx++)
				texture.SetPixel(xx, yy, color);
		}
	}

	private static Color ColorFromHex(uint rgba) {
		float r = ((rgba >> 24) & 0xff) / 255f;
		float g = ((rgba >> 16) & 0xff) / 255f;
		float b = ((rgba >> 8) & 0xff) / 255f;
		float a = (rgba & 0xff) / 255f;
		return new Color(r, g, b, a);
	}
}
