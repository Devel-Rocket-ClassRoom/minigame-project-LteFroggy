using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaShopPanelController : MonoBehaviour {
	private const string PanelName = "MetaShopPanel";
	private const string ResourcePath = "UI/MetaShopPanel";

	private static MetaShopPanelController _instance;

	private RectTransform _listRoot;
	private TextMeshProUGUI _statusText;
	private TextMeshProUGUI _goldText;
	private Button _closeButton;
	private FirebaseMetaProgressManager _metaProgressManager;
	private bool _initialized;

	public static void Show() {
		if (_instance != null) {
			_instance.gameObject.SetActive(true);
			_instance.Initialize();
			return;
		}

		GameObject prefab = Resources.Load<GameObject>(ResourcePath);
		GameObject go = prefab != null
			? Instantiate(prefab)
			: new GameObject(PanelName);
		go.name = PanelName;

		_instance = go.GetComponent<MetaShopPanelController>();
		if (_instance == null)
			_instance = go.AddComponent<MetaShopPanelController>();
		_instance.Initialize();
	}

	private void Awake() {
		if (_instance == null)
			_instance = this;
	}

	private void OnDestroy() {
		if (_closeButton != null)
			_closeButton.onClick.RemoveListener(Hide);
		if (_instance == this)
			_instance = null;
	}

	private void Initialize() {
		if (!_initialized) {
			BindPrefabLayout();
			bool hasPrefabLayout = _listRoot != null && _goldText != null && _statusText != null;
			if (!hasPrefabLayout)
				Build();
			else if (_closeButton != null)
				_closeButton.onClick.AddListener(Hide);
			_initialized = true;
		}

		Refresh();
	}

	private void BindPrefabLayout() {
		_listRoot = FindChildRect("Content");
		_statusText = FindChildComponent<TextMeshProUGUI>("StatusText");
		_goldText = FindChildComponent<TextMeshProUGUI>("GoldText");
		_closeButton = FindChildComponent<Button>("CloseButton");
	}

	private void Build() {
		var canvasObject = new GameObject("Meta Shop Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		canvasObject.transform.SetParent(transform, false);

		Canvas canvas = canvasObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 180;

		CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.matchWidthOrHeight = 0.5f;

		RectTransform root = (RectTransform)canvasObject.transform;
		CreatePanel("Dimmer", root, ColorFromHex(0x00000099), Vector2.zero, Vector2.one);

		Image panel = CreatePanel("Shop Panel", root, ColorFromHex(0x12090df4), new Vector2(0.16f, 0.12f), new Vector2(0.84f, 0.88f));
		RectTransform panelRect = panel.rectTransform;

		CreateText("Title", panelRect, "유물 상점", 42f, FontStyles.Bold, TextAlignmentOptions.Left, ColorFromHex(0xffdcc3ff), new Vector2(0.05f, 0.88f), new Vector2(0.45f, 0.97f));
		_goldText = CreateText("Gold", panelRect, "", 28f, FontStyles.Bold, TextAlignmentOptions.Right, ColorFromHex(0xffd36eff), new Vector2(0.58f, 0.89f), new Vector2(0.84f, 0.96f));
		_statusText = CreateText("Status", panelRect, "", 23f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xded5c9ff), new Vector2(0.05f, 0.05f), new Vector2(0.66f, 0.1f));

		_closeButton = CreateButton("CloseButton", panelRect, "닫기", new Vector2(0.86f, 0.89f), new Vector2(0.95f, 0.96f), Hide);
		_closeButton.interactable = true;

		GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
		viewport.transform.SetParent(panelRect, false);
		RectTransform viewportRect = (RectTransform)viewport.transform;
		viewportRect.anchorMin = new Vector2(0.05f, 0.13f);
		viewportRect.anchorMax = new Vector2(0.95f, 0.85f);
		viewportRect.offsetMin = Vector2.zero;
		viewportRect.offsetMax = Vector2.zero;
		Image viewportImage = viewport.GetComponent<Image>();
		viewportImage.color = ColorFromHex(0x080507aa);
		viewport.GetComponent<Mask>().showMaskGraphic = false;

		GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
		content.transform.SetParent(viewport.transform, false);
		_listRoot = (RectTransform)content.transform;
		_listRoot.anchorMin = new Vector2(0f, 1f);
		_listRoot.anchorMax = new Vector2(1f, 1f);
		_listRoot.pivot = new Vector2(0.5f, 1f);
		_listRoot.offsetMin = Vector2.zero;
		_listRoot.offsetMax = Vector2.zero;

		VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>();
		layout.padding = new RectOffset(12, 12, 12, 12);
		layout.spacing = 10f;
		layout.childControlHeight = true;
		layout.childControlWidth = true;
		layout.childForceExpandHeight = false;
		layout.childForceExpandWidth = true;

		ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
		fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		ScrollRect scrollRect = panel.gameObject.AddComponent<ScrollRect>();
		scrollRect.viewport = viewportRect;
		scrollRect.content = _listRoot;
		scrollRect.horizontal = false;

	}

	private void Refresh() {
		_metaProgressManager = FirebaseBootstrapper.Instance != null
			? FirebaseBootstrapper.Instance.MetaProgressManager
			: null;

		if (_listRoot == null)
			return;

		ClearList();
		UpdateGoldText();

		if (_metaProgressManager == null) {
			SetStatus("Firebase 메타 진행 정보를 사용할 수 없습니다.");
			return;
		}

		if (!_metaProgressManager.HasData) {
			SetStatus("메타 진행 정보를 불러오는 중입니다.");
			LoadThenRefresh().Forget();
			return;
		}

		foreach (RelicBase relic in GameContentCatalog.AllLoadoutRelics)
			CreateRelicRow(relic);

		SetStatus("구매한 유물은 다음 런 로드아웃에 표시됩니다.");
	}

	private async UniTaskVoid LoadThenRefresh() {
		var (success, error) = await _metaProgressManager.LoadOrCreateMetaProgress();
		if (!success) {
			SetStatus(error);
			return;
		}

		Refresh();
	}

	private void CreateRelicRow(RelicBase relic) {
		GameObject row = new GameObject(relic.relicId, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
		row.transform.SetParent(_listRoot, false);

		Image background = row.GetComponent<Image>();
		background.color = ColorFromHex(0x1f141aff);

		LayoutElement rowLayout = row.GetComponent<LayoutElement>();
		rowLayout.preferredHeight = 116f;

		HorizontalLayoutGroup group = row.GetComponent<HorizontalLayoutGroup>();
		group.padding = new RectOffset(14, 14, 10, 10);
		group.spacing = 12f;
		group.childAlignment = TextAnchor.MiddleLeft;
		group.childControlHeight = true;
		group.childControlWidth = true;
		group.childForceExpandHeight = true;
		group.childForceExpandWidth = false;

		Image icon = CreateIcon(row.transform, relic.icon);
		icon.gameObject.AddComponent<LayoutElement>().preferredWidth = 82f;

		TextMeshProUGUI description = CreateText("Description", row.transform, BuildRelicDescription(relic), 22f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf4e4d8ff), Vector2.zero, Vector2.one);
		LayoutElement descLayout = description.gameObject.AddComponent<LayoutElement>();
		descLayout.preferredWidth = 820f;
		descLayout.flexibleWidth = 1f;

		int price = FirebaseMetaProgressManager.GetRelicPrice(relic);
		bool canAfford = _metaProgressManager.Current.gold >= price;
		bool unlocked = _metaProgressManager.IsRelicUnlocked(relic);
		string buttonText = unlocked ? "보유" : $"{FirebaseMetaProgressManager.GetRelicPrice(relic)} G";
		Button buyButton = CreateButton("Buy Button", row.transform, buttonText, Vector2.zero, Vector2.one, () => OnBuyClicked(relic).Forget());
		LayoutElement buttonLayout = buyButton.gameObject.AddComponent<LayoutElement>();
		buttonLayout.preferredWidth = 150f;
		buyButton.interactable = !unlocked && canAfford;

		Image buttonBackground = buyButton.GetComponent<Image>();
		TextMeshProUGUI buttonLabel = buyButton.GetComponentInChildren<TextMeshProUGUI>();
		bool unavailable = !unlocked && !canAfford;
		if (buttonLabel != null)
			buttonLabel.text = unlocked
				? "보유"
				: unavailable ? $"골드 부족\n{price} G 필요" : $"구매\n{price} G";
		if (buttonBackground != null)
			buttonBackground.color = unlocked
				? ColorFromHex(0x343038ff)
				: unavailable ? ColorFromHex(0x29242aff) : ColorFromHex(0x3b1018ff);
		if (buttonLabel != null && unavailable)
			buttonLabel.color = ColorFromHex(0xaaa0a8ff);
	}

	private async UniTaskVoid OnBuyClicked(RelicBase relic) {
		if (_metaProgressManager == null)
			return;

		var (success, error) = await _metaProgressManager.TryPurchaseRelic(relic);
		SetStatus(success ? $"{relic.displayName} 구매 완료" : error);
		Refresh();
	}

	private string BuildRelicDescription(RelicBase relic) {
		string rarity = StringTableManager.StringTable[relic.rarity.ToString()];
		return $"{relic.displayName}\n<size=18>{rarity} / 코스트 {relic.cost}</size>\n<size=19>{relic.effectDescription}</size>";
	}

	private void UpdateGoldText() {
		int gold = _metaProgressManager != null && _metaProgressManager.Current != null
			? _metaProgressManager.Current.gold
			: 0;
		if (_goldText != null)
			_goldText.text = $"보유 골드 {gold}";
	}

	private void SetStatus(string message) {
		if (_statusText != null)
			_statusText.text = message;
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private RectTransform FindChildRect(string objectName) {
		RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);
		foreach (RectTransform rect in rects) {
			if (rect.gameObject.name == objectName)
				return rect;
		}

		return null;
	}

	private T FindChildComponent<T>(string objectName) where T : Component {
		T[] components = GetComponentsInChildren<T>(true);
		foreach (T component in components) {
			if (component.gameObject.name == objectName)
				return component;
		}

		return null;
	}

	private void ClearList() {
		for (int i = _listRoot.childCount - 1; i >= 0; i--)
			Destroy(_listRoot.GetChild(i).gameObject);
	}

	private static Image CreatePanel(string objectName, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		go.transform.SetParent(parent, false);

		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		Image image = go.GetComponent<Image>();
		image.color = color;
		image.raycastTarget = true;
		return image;
	}

	private static Image CreateIcon(Transform parent, Sprite sprite) {
		GameObject go = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		go.transform.SetParent(parent, false);
		Image image = go.GetComponent<Image>();
		image.sprite = sprite;
		image.preserveAspect = true;
		image.color = sprite != null ? Color.white : ColorFromHex(0x4d3940ff);
		return image;
	}

	private static TextMeshProUGUI CreateText(string objectName, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		go.transform.SetParent(parent, false);

		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
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

	private static Button CreateButton(string objectName, Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick) {
		GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		go.transform.SetParent(parent, false);

		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		Image image = go.GetComponent<Image>();
		image.color = ColorFromHex(0x3b1018ff);

		Button button = go.GetComponent<Button>();
		button.onClick.AddListener(onClick);

		CreateText("Text", rect, text, 21f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), Vector2.zero, Vector2.one);
		return button;
	}

	private static Color ColorFromHex(uint rgba) {
		float r = ((rgba >> 24) & 0xff) / 255f;
		float g = ((rgba >> 16) & 0xff) / 255f;
		float b = ((rgba >> 8) & 0xff) / 255f;
		float a = (rgba & 0xff) / 255f;
		return new Color(r, g, b, a);
	}
}
