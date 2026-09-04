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
	private Button _guestLoginButton;
	private bool _guestDataWarningAcknowledged;

	public event Action<string, string> OnEmailSignInClicked;
	public event Action<string, string> OnEmailSignUpClicked;
	public event Action OnAnonymousSignInClicked;

	private void OnEnable() {
		_guestDataWarningAcknowledged = false;
		EnsureGuestLoginButton();
		ApplySettings();
		ClearStatus();

		if (_loginButton != null) _loginButton.onClick.AddListener(OnLoginClicked);
		if (_signUpButton != null) _signUpButton.onClick.AddListener(OnSignUpClicked);
		if (_guestLoginButton != null) _guestLoginButton.onClick.AddListener(OnGuestLoginClicked);
		if (_closeButton != null) _closeButton.onClick.AddListener(Close);
	}

	private void OnDisable() {
		_guestDataWarningAcknowledged = false;
		if (_loginButton != null) _loginButton.onClick.RemoveListener(OnLoginClicked);
		if (_signUpButton != null) _signUpButton.onClick.RemoveListener(OnSignUpClicked);
		if (_guestLoginButton != null) _guestLoginButton.onClick.RemoveListener(OnGuestLoginClicked);
		if (_closeButton != null) _closeButton.onClick.RemoveListener(Close);
	}

	private void ApplySettings() {
		if (_emailFormRoot != null) _emailFormRoot.SetActive(true);
		if (_emailActionRoot != null) _emailActionRoot.SetActive(true);
		if (_loginButton != null) _loginButton.gameObject.SetActive(true);
		if (_signUpButton != null) _signUpButton.gameObject.SetActive(true);
		if (_guestLoginButton != null) _guestLoginButton.gameObject.SetActive(true);
		SetEmailActionHeight(174f);

		if (_descriptionText != null) {
			_descriptionText.text = "이메일 계정으로 로그인하거나 새 계정을 만듭니다.";
		}

		SetButtonText(_loginButton, "로그인");
		SetButtonText(_signUpButton, "회원가입");
		SetButtonText(_guestLoginButton, "게스트로 시작");
	}

	public void ConfigureForEmailLink() {
		if (_descriptionText != null)
			_descriptionText.text = "현재 익명 계정의 진행 데이터를 유지한 채 이메일 계정으로 연결합니다.";
		if (_loginButton != null)
			_loginButton.gameObject.SetActive(false);
		if (_signUpButton != null)
			_signUpButton.gameObject.SetActive(true);
		SetButtonText(_signUpButton, "이메일 연결");
		if (_guestLoginButton != null) _guestLoginButton.gameObject.SetActive(false);
		SetEmailActionHeight(112f);
		ClearStatus();
	}

	public void ConfigureForEmailAuth() {
		ApplySettings();
		ClearStatus();
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

	private static void SetButtonText(Button button, string text) {
		if (button == null)
			return;

		TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
		if (label != null)
			label.text = text;
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
	private void EnsureGuestLoginButton() {
		if (_guestLoginButton != null || _loginButton == null)
			return;

		_guestLoginButton = Instantiate(_loginButton, _loginButton.transform.parent);
		_guestLoginButton.gameObject.name = "GuestLoginButton";
		_guestLoginButton.transform.SetSiblingIndex(_loginButton.transform.GetSiblingIndex() + 1);
		_guestLoginButton.onClick.RemoveAllListeners();

		Image background = _guestLoginButton.GetComponent<Image>();
		if (background != null)
			background.color = new Color(0.16f, 0.28f, 0.48f, 1f);
	}

	private void SetEmailActionHeight(float height) {
		if (_emailActionRoot != null && _emailActionRoot.transform is RectTransform rectTransform)
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
	}

	private void OnGuestLoginClicked() {
		if (!_guestDataWarningAcknowledged) {
			_guestDataWarningAcknowledged = true;
			ShowError("주의: 게스트 계정은 기기를 변경하거나 앱 데이터를 삭제하면 진행 데이터를 복구할 수 없습니다.\n계속하려면 게스트로 시작 버튼을 한 번 더 눌러주세요.");
			SetButtonText(_guestLoginButton, "경고 확인 후 다시 클릭");
			return;
		}

		ClearStatus();
		ShowInfo("게스트 로그인 중...");
		OnAnonymousSignInClicked?.Invoke();
	}

	private void Close() {
		gameObject.SetActive(false);
	}
}
