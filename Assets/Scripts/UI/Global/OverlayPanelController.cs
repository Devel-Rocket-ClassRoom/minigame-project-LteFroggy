using UnityEngine;
using UnityEngine.UI;

public class OverlayPanelController : MonoBehaviour {
	[Header("=== 닫기 버튼 ===")]
	[SerializeField] private Button _closeButton;

	private void Awake() {
		_closeButton.onClick.AddListener(Close);
	}
	
	public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

	public void Open() => gameObject.SetActive(true);
	private void Close() => gameObject.SetActive(false);
}
