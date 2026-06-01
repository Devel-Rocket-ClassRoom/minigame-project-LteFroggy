using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanelController : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	[SerializeField] private Image _iconImage;

	public void SetContent(string title, string description, Sprite icon = null) {
		_titleText.text = title;
		_descriptionText.text = description;

		// 텍스트 세팅 할 때, 내부 키워드 존재하는지 확인해야 함
		if (_iconImage != null) {
			_iconImage.sprite = icon;
			_iconImage.gameObject.SetActive(icon != null);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}
}
