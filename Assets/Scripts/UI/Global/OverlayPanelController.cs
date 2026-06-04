using System;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPanelController : MonoBehaviour {
	public static event Action<bool> OnVisibilityChanged;

	[Header("=== 닫기 버튼 ===")]
	[SerializeField] private Button _closeButton;

	private void Awake() {
		_closeButton.onClick.AddListener(Close);
	}

	private void OnEnable()  => OnVisibilityChanged?.Invoke(true);
	private void OnDisable() => OnVisibilityChanged?.Invoke(false);

	public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);

	public void Open() => gameObject.SetActive(true);
	public void Close() => gameObject.SetActive(false);
}
