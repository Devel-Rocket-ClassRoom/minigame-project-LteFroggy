using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeLegendController : MonoBehaviour {
	private const float k_LegendWidth = 600f;
	private const float k_LegendHeight = 92f;
	private const float k_BottomOffset = 36f;
	private const float k_ItemWidth = 82f;
	private const float k_IconSize = 48f;
	private const float k_LabelHeight = 26f;
	private const float k_ItemSpacing = 12f;

	private static readonly LegendEntry[] k_Entries = {
		new(MapNodeType.Start, "\uC2DC\uC791"),
		new(MapNodeType.Battle, "\uC804\uD22C"),
		new(MapNodeType.Event, "\uC774\uBCA4\uD2B8"),
		new(MapNodeType.Rest, "\uD734\uC2DD"),
		new(MapNodeType.Treasure, "\uBCF4\uBB3C"),
		new(MapNodeType.Boss, "\uBCF4\uC2A4")
	};

	[SerializeField] private TMP_FontAsset _fontAsset;

	private bool _built;

	private void Awake() {
		ConfigureRoot();
		BuildLegend();
	}

	private void OnEnable() {
		if (_built) return;

		ConfigureRoot();
		BuildLegend();
	}

	private void ConfigureRoot() {
		var rectTransform = (RectTransform)transform;
		rectTransform.anchorMin = new Vector2(0.5f, 0f);
		rectTransform.anchorMax = new Vector2(0.5f, 0f);
		rectTransform.pivot = new Vector2(0.5f, 0f);
		rectTransform.anchoredPosition = new Vector2(0f, k_BottomOffset);
		rectTransform.sizeDelta = new Vector2(k_LegendWidth, k_LegendHeight);

		var layoutGroup = GetComponent<HorizontalLayoutGroup>();
		if (layoutGroup == null) layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();

		layoutGroup.childAlignment = TextAnchor.MiddleCenter;
		layoutGroup.spacing = k_ItemSpacing;
		layoutGroup.childControlWidth = false;
		layoutGroup.childControlHeight = false;
		layoutGroup.childForceExpandWidth = false;
		layoutGroup.childForceExpandHeight = false;
	}

	private void BuildLegend() {
		var mapConfig = GamePlayData.Instance.MapGeneratingConfig;
		if (mapConfig == null) return;

		ClearChildren();

		foreach (var entry in k_Entries) {
			var nodeConfig = mapConfig.GetConfig(entry.Type);
			CreateLegendItem(nodeConfig.Icon, entry.Label);
		}

		_built = true;
	}

	private void ClearChildren() {
		for (int i = transform.childCount - 1; i >= 0; i--) {
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	private void CreateLegendItem(Sprite icon, string label) {
		var itemObject = new GameObject($"{label}Item", typeof(RectTransform));
		itemObject.transform.SetParent(transform, false);

		var itemRect = (RectTransform)itemObject.transform;
		itemRect.sizeDelta = new Vector2(k_ItemWidth, k_LegendHeight);

		var layoutElement = itemObject.AddComponent<LayoutElement>();
		layoutElement.preferredWidth = k_ItemWidth;
		layoutElement.preferredHeight = k_LegendHeight;

		var verticalLayout = itemObject.AddComponent<VerticalLayoutGroup>();
		verticalLayout.childAlignment = TextAnchor.MiddleCenter;
		verticalLayout.spacing = 2f;
		verticalLayout.padding = new RectOffset(0, 0, 4, 0);
		verticalLayout.childControlWidth = false;
		verticalLayout.childControlHeight = false;
		verticalLayout.childForceExpandWidth = false;
		verticalLayout.childForceExpandHeight = false;

		CreateIcon(itemObject.transform, icon);
		CreateLabel(itemObject.transform, label);
	}

	private static void CreateIcon(Transform parent, Sprite icon) {
		var iconObject = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		iconObject.transform.SetParent(parent, false);

		var iconRect = (RectTransform)iconObject.transform;
		iconRect.sizeDelta = new Vector2(k_IconSize, k_IconSize);

		var layoutElement = iconObject.AddComponent<LayoutElement>();
		layoutElement.preferredWidth = k_IconSize;
		layoutElement.preferredHeight = k_IconSize;

		var image = iconObject.GetComponent<Image>();
		image.sprite = icon;
		image.preserveAspect = true;
		image.raycastTarget = false;
	}

	private void CreateLabel(Transform parent, string label) {
		var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		labelObject.transform.SetParent(parent, false);

		var labelRect = (RectTransform)labelObject.transform;
		labelRect.sizeDelta = new Vector2(k_ItemWidth, k_LabelHeight);

		var layoutElement = labelObject.AddComponent<LayoutElement>();
		layoutElement.preferredWidth = k_ItemWidth;
		layoutElement.preferredHeight = k_LabelHeight;

		var text = labelObject.GetComponent<TextMeshProUGUI>();
		text.text = label;
		if (_fontAsset != null) text.font = _fontAsset;
		text.alignment = TextAlignmentOptions.Center;
		text.color = new Color(1f, 0.93f, 0.78f, 1f);
		text.fontSize = 18f;
		text.enableAutoSizing = true;
		text.fontSizeMin = 12f;
		text.fontSizeMax = 18f;
		text.overflowMode = TextOverflowModes.Ellipsis;
		text.raycastTarget = false;
	}

	private readonly struct LegendEntry {
		public readonly MapNodeType Type;
		public readonly string Label;

		public LegendEntry(MapNodeType type, string label) {
			Type = type;
			Label = label;
		}
	}
}
