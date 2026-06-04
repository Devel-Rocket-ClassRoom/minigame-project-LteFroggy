using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private string _title;
	private string _description;
	private bool _isHovering;

	public void SetContent(string title, string description) {
		_title = title;
		_description = description;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (string.IsNullOrEmpty(_title) || string.IsNullOrEmpty(_description)) return;

		_isHovering = true;
		DescriptionSystem.ProcessDescriptionPanel(_title, _description, (RectTransform)transform);
	}

	public void OnPointerExit(PointerEventData eventData) {
		_isHovering = false;
		DescriptionSystem.Hide();
	}

	private void OnDisable() {
		if (!_isHovering) return;

		DescriptionSystem.Hide();
		_isHovering = false;
	}
}
