using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour, ILoginManager {

	[SerializeField] private TMP_InputField _emailInput;
	[SerializeField] private TMP_InputField _passwordInput;
	[SerializeField] private GameObject _emailFormRoot;
	[SerializeField] private GameObject _emailActionRoot;
	[SerializeField] private Button _loginButton;
	[SerializeField] private Button _signUpButton;
	[SerializeField] private Button _closeButton;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	[SerializeField] private TextMeshProUGUI _statusText;

	public event Action<string, string> OnEmailSignInClicked;
	public event Action<string, string> OnEmailSignUpClicked;

	private void OnEnable() {
		ApplySettings();
		ClearStatus();

		if (_loginButton != null) _loginButton.onClick.AddListener(OnLoginClicked);
		if (_signUpButton != null) _signUpButton.onClick.AddListener(OnSignUpClicked);
		if (_closeButton != null) _closeButton.onClick.AddListener(Close);
	}

	private void OnDisable() {
		if (_loginButton != null) _loginButton.onClick.RemoveListener(OnLoginClicked);
		if (_signUpButton != null) _signUpButton.onClick.RemoveListener(OnSignUpClicked);
		if (_closeButton != null) _closeButton.onClick.RemoveListener(Close);
	}

	private void ApplySettings() {
		if (_emailFormRoot != null) _emailFormRoot.SetActive(true);
		if (_emailActionRoot != null) _emailActionRoot.SetActive(true);

		if (_descriptionText != null) {
			_descriptionText.text = "이메일 계정으로 로그인하거나 새 계정을 만듭니다.";
		}
	}

	public void ShowError(string message) {
		SetStatus(message, new Color(0.95f, 0.42f, 0.36f, 1f));
	}

	public void ShowInfo(string message) {
		SetStatus(message, new Color(0.72f, 0.82f, 0.95f, 1f));
	}

	public void ClearStatus() {
		SetStatus(string.Empty, new Color(0.95f, 0.42f, 0.36f, 1f));
	}

	private void SetStatus(string message, Color color) {
		if (_statusText == null) return;

		_statusText.text = message;
		_statusText.color = color;
	}

	private void OnLoginClicked() {
		ClearStatus();
		ShowInfo("로그인 중...");
		OnEmailSignInClicked?.Invoke(_emailInput.text, _passwordInput.text);
	}

	private void OnSignUpClicked() {
		ClearStatus();
		ShowInfo("회원가입 중...");
		OnEmailSignUpClicked?.Invoke(_emailInput.text, _passwordInput.text);
	}

	private void Close() {
		gameObject.SetActive(false);
	}
}
