using UnityEngine;
using UnityEngine.UI;

public class OverlayPanelController : MonoBehaviour {
	[Header("=== 닫기 버튼 ===")]
	[SerializeField] private Button _closeButton;

	private void Awake() {
		_closeButton.onClick.AddListener(Close);
	}

	public void Open() => gameObject.SetActive(true);
	public void Close() => gameObject.SetActive(false);
}
