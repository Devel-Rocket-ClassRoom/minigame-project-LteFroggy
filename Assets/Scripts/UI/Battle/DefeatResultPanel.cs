using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefeatResultPanel : MonoBehaviour {
	public static void Show() {
		var go = new GameObject("DefeatResultPanel");
		go.AddComponent<DefeatResultPanel>().Build();
	}

	private void Build() {
		var canvasObject = new GameObject("Defeat Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
		canvasObject.transform.SetParent(transform, false);

		var canvas = canvasObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 200;

		var scaler = canvasObject.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.matchWidthOrHeight = 0.5f;

		var root = (RectTransform)canvasObject.transform;

		CreatePanel("Dimmer", root, ColorFromHex(0x000000b0), Vector2.zero, Vector2.one);

		var panel = CreatePanel("Result Panel", root, ColorFromHex(0x09070bf2), new Vector2(0.27f, 0.22f), new Vector2(0.73f, 0.76f));
		var panelRect = (RectTransform)panel.transform;

		CreateText("Title", panelRect, "패배하였습니다", 42f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xff4444ff), new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.91f));
		var summaryText = CreateText("Summary", panelRect, "", 26f, FontStyles.Normal, TextAlignmentOptions.Left, ColorFromHex(0xf4e9ddff), new Vector2(0.1f, 0.28f), new Vector2(0.9f, 0.71f));
		summaryText.text = BuildRunSummary();

		CreateButton("Return Button", panelRect, "처음으로", new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.22f), ReturnToMainMenu);
	}

	private void ReturnToMainMenu() {
		GamePlayData.Instance.Reset();
		UISceneBootstrapper.Instance.TransitionTo("MainScene");
	}

	private static string BuildRunSummary() {
		var data = GamePlayData.Instance;
		var builder = new StringBuilder();
		builder.AppendLine($"최종 덱 카드 수: {data.Deck.Count}");
		builder.AppendLine($"획득 유물 수: {data.Relics.Count}");
		builder.AppendLine($"남은 HP: {data.CurrentHealth}/{data.MaxHealth}");
		builder.AppendLine($"보유 골드: {data.Gold}");
		builder.Append("보유 유물: ");

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

	private static Image CreatePanel(string objectName, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax) {
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

	private static TextMeshProUGUI CreateText(string objectName, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax) {
		var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		go.transform.SetParent(parent, false);

		var rect = (RectTransform)go.transform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;

		var label = go.GetComponent<TextMeshProUGUI>();
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
		button.onClick.AddListener(onClick);

		CreateText("Text", rect, text, 28f, FontStyles.Bold, TextAlignmentOptions.Center, ColorFromHex(0xffe3d5ff), Vector2.zero, Vector2.one);
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
