using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicIconController : MonoBehaviour {
	[Header("=== 유물 아이콘 ===")]
	[SerializeField] private Image _icon;

	private const float CardWidth = 74f;
	private const float CardHeight = 82f;
	private const float IconSize = 52f;

	private Image _background;
	private TextMeshProUGUI _nameText;

	public void Set(RelicBase relic) {
		Set(relic, null);
	}

	public void Set(RelicBase relic, TMP_FontAsset fontAsset) {
		EnsureVisuals();

		_icon.sprite = relic.icon;
		_icon.preserveAspect = true;
		_icon.color = Color.white;
		if (fontAsset != null) _nameText.font = fontAsset;
		_nameText.text = relic.displayName;
	}

	private void EnsureVisuals() {
		var rectTransform = (RectTransform)transform;
		rectTransform.sizeDelta = new Vector2(CardWidth, CardHeight);

		var layoutElement = GetComponent<LayoutElement>();
		if (layoutElement == null) layoutElement = gameObject.AddComponent<LayoutElement>();
		layoutElement.preferredWidth = CardWidth;
		layoutElement.preferredHeight = CardHeight;

		_background = GetComponent<Image>();
		if (_background == null) _background = gameObject.AddComponent<Image>();
		_background.color = new Color(0.08f, 0.07f, 0.09f, 0.88f);

		var outline = GetComponent<Outline>();
		if (outline == null) outline = gameObject.AddComponent<Outline>();
		outline.effectColor = new Color(0.95f, 0.78f, 0.38f, 0.85f);
		outline.effectDistance = new Vector2(2f, -2f);

		if (_icon == null || _icon.transform == transform) {
			_icon = GetOrCreateChildImage("Icon");
		}

		ConfigureIconRect((RectTransform)_icon.transform);
		_nameText = GetOrCreateNameText();
	}

	private Image GetOrCreateChildImage(string childName) {
		Transform child = transform.Find(childName);
		if (child == null) {
			var childObject = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			childObject.transform.SetParent(transform, false);
			child = childObject.transform;
		}

		return child.GetComponent<Image>();
	}

	private static void ConfigureIconRect(RectTransform iconRect) {
		iconRect.anchorMin = new Vector2(0.5f, 1f);
		iconRect.anchorMax = new Vector2(0.5f, 1f);
		iconRect.pivot = new Vector2(0.5f, 1f);
		iconRect.anchoredPosition = new Vector2(0f, -8f);
		iconRect.sizeDelta = new Vector2(IconSize, IconSize);
	}

	private TextMeshProUGUI GetOrCreateNameText() {
		Transform child = transform.Find("Name");
		if (child == null) {
			var childObject = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
			childObject.transform.SetParent(transform, false);
			child = childObject.transform;
		}

		var text = child.GetComponent<TextMeshProUGUI>();
		var textRect = (RectTransform)child;
		textRect.anchorMin = new Vector2(0f, 0f);
		textRect.anchorMax = new Vector2(1f, 0f);
		textRect.pivot = new Vector2(0.5f, 0f);
		textRect.anchoredPosition = new Vector2(0f, 6f);
		textRect.sizeDelta = new Vector2(-8f, 20f);

		text.alignment = TextAlignmentOptions.Center;
		text.color = new Color(0.96f, 0.9f, 0.72f, 1f);
		text.fontSize = 13f;
		text.enableAutoSizing = true;
		text.fontSizeMin = 8f;
		text.fontSizeMax = 13f;
		text.overflowMode = TextOverflowModes.Ellipsis;
		text.raycastTarget = false;

		return text;
	}
}
