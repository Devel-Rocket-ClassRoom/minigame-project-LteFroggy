using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	private float _originalScale;
	private float _accentScale = 1.5f;
	private bool _isFocused;
	private float _hoverYDiff;
	private int _childIndex;

	private void Awake() {
		_originalScale = transform.localScale.x;
		_hoverYDiff = 100f;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		// 크기 조절, 제일 마지막 자식으로 위치 변경
		_isFocused = true;
		_childIndex = transform.GetSiblingIndex();
		
		transform.SetAsLastSibling();
		transform.localScale = new Vector3(_accentScale, _accentScale, _accentScale);
		transform.position += new Vector3(0, _hoverYDiff, 0);
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		_isFocused = false;
		
		transform.SetSiblingIndex(_childIndex);
		transform.localScale = new Vector3(_originalScale, _originalScale, _originalScale);
		transform.position -= new Vector3(0, _hoverYDiff, 0);
	}
}