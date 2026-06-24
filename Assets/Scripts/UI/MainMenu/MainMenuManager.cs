using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	private const string LogoutButtonName = "LogoutButton";

	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _newRunButton;
	[SerializeField] private Button _statsButton;
	[SerializeField] private Button _loginButton;
	[SerializeField] private Button _logoutButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private GameObject _loadoutPanel;
	[SerializeField] private LoginManager _loginPanel;
	[SerializeField] private UserStatsPanelController _statsPanel;
	[SerializeField] private Image _loginStatusPanel;

	private async void OnEnable() {
		_logoutButton = EnsureLogoutButton();

		_quitButton.onClick.AddListener(QuitGame);
		_newRunButton.onClick.AddListener(OnNewRunClicked);
		if (_statsButton != null)
			_statsButton.onClick.AddListener(OpenStats);
		_loginButton.onClick.AddListener(OpenLogin);
		if (_logoutButton != null)
			_logoutButton.onClick.AddListener(OnLogoutClicked);

		if (RequiresEmailLogin())
			ShowLoginButton(false);
		else
			ShowStartButton(false);

		PrepareButtonAsync().Forget();
	}

	private void OnDisable() {
		_newRunButton.onClick.RemoveListener(OnNewRunClicked);
		if (_statsButton != null)
			_statsButton.onClick.RemoveListener(OpenStats);
		_loginButton.onClick.RemoveListener(OpenLogin);
		if (_logoutButton != null)
			_logoutButton.onClick.RemoveListener(OnLogoutClicked);
		_quitButton.onClick.RemoveListener(QuitGame);
		UnsubscribeLoginPanelEvents();
	}
	
	private async UniTaskVoid PrepareButtonAsync() {
		// Firebase 초기화와 필요한 로그인이 끝난 뒤 메뉴 버튼을 활성화합니다.
		await UniTask.WaitUntil(() => FirebaseBootstrapper.Instance != null);

		FirebaseBootstrapper bootstrapper = FirebaseBootstrapper.Instance;
		InitState state = await bootstrapper.WaitForInitializationAsync();

		if (state == InitState.Ready) {
			if (!RequiresEmailLogin()) {
				bool isLoggedIn = bootstrapper.AuthManager != null && bootstrapper.AuthManager.IsLoggedIn;
				if (!isLoggedIn && bootstrapper.AuthManager != null) {
					var (result, error) = await bootstrapper.AuthManager.SignIn();
					isLoggedIn = result;
					if (!result)
						Debug.LogWarning($"[MainMenuManager] 익명 로그인 실패: {error}");
				}

				ShowStartButton(isLoggedIn);
				return;
			}

			if (bootstrapper.AuthManager != null && bootstrapper.AuthManager.IsLoggedIn) {
				ShowStartButton(true);
				return;
			}

			ShowLoginButton(true);
			return;
		}

		ShowStartButton(state == InitState.Disabled);
	}
	
	private void OnNewRunClicked() {
		_loadoutPanel.SetActive(true);
		if (_statsPanel != null)
			_statsPanel.Hide();
		if (_loadoutPanel.TryGetComponent(out LoadoutManager loadoutManager))
			loadoutManager.RefreshRelicList();
	}

	private void OpenStats() {
		if (_loadoutPanel != null)
			_loadoutPanel.SetActive(false);
		if (_loginPanel != null)
			_loginPanel.gameObject.SetActive(false);
		if (_statsPanel != null)
			_statsPanel.Show();
	}
	
	private void OpenLogin() {
		// 열릴 때, 이미 로그인 되어이쓴 상태라면 패널 열 필요 없이 바로 로드아웃으로
		if (IsLoggedIn()) {
			Debug.Log("[MainMenuManager] 이미 로그인되어있어 바로 로드아웃 패널로 연결됩니다.");
			LoginSuccess();
			return;
		}
		
		_loadoutPanel.SetActive(false);
		if (_loginPanel == null) {
			Debug.LogError("[MainMenuManager] 로그인 패널이 연결되어 있지 않습니다.");
			return;
		}

		UnsubscribeLoginPanelEvents();
		_loginPanel.OnEmailSignInClicked += OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked += OnEmailSignUpClicked;

		_loginPanel.gameObject.SetActive(true);
	}
	
	private async void OnEmailSignUpClicked(string email, string password) {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignUp(email, password);
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private async void OnEmailSignInClicked(string email, string password) {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignIn(email, password);
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private void LoginSuccess() {
		UnsubscribeLoginPanelEvents();
		if (_loginPanel != null)
			_loginPanel.gameObject.SetActive(false);

		ShowStartButton();
	}

	private void OnLogoutClicked() {
		FirebaseAuthManager authManager = GetAuthManager();
		if (authManager == null) {
			Debug.LogWarning("[MainMenuManager] Firebase 인증 관리자가 없어 로그아웃을 건너뜁니다.");
			ShowLoginButton(false);
			return;
		}

		if (_logoutButton != null)
			_logoutButton.interactable = false;

		var (success, error) = authManager.SignOut();
		if (!success) {
			Debug.LogWarning($"[MainMenuManager] 로그아웃 실패: {error}");
			ShowStartButton();
			return;
		}

		if (_loginPanel != null)
			_loginPanel.gameObject.SetActive(false);

		if (_loadoutPanel != null)
			_loadoutPanel.SetActive(false);

		ShowLoginButton(true);
	}

	private void ShowLoginButton(bool interactable) {
		_newRunButton.interactable = false;
		_newRunButton.gameObject.SetActive(false);
		SetStatsButton(false);

		_loginButton.interactable = interactable;
		_loginButton.gameObject.SetActive(true);

		SetLogoutButtonVisible(false);
		HideLoginStatus();
	}

	private void ShowStartButton(bool interactable = true) {
		_loginButton.gameObject.SetActive(false);
		_loginButton.interactable = false;

		_newRunButton.interactable = interactable;
		_newRunButton.gameObject.SetActive(true);
		SetStatsButton(interactable);

		SetLogoutButtonVisible(interactable && RequiresEmailLogin() && IsLoggedIn());
		HideLoginStatus();
	}

	private void SetStatsButton(bool visible) {
		if (_statsButton == null)
			return;

		_statsButton.interactable = visible;
		_statsButton.gameObject.SetActive(visible);
	}

	private void HideLoginStatus() {
		if (_loginStatusPanel == null)
			return;

		_loginStatusPanel.gameObject.SetActive(false);
	}

	private bool IsLoggedIn() {
		FirebaseAuthManager authManager = GetAuthManager();
		return authManager != null && authManager.IsLoggedIn;
	}

	private FirebaseAuthManager GetAuthManager() {
		FirebaseBootstrapper bootstrapper = FirebaseBootstrapper.Instance;
		return bootstrapper != null ? bootstrapper.AuthManager : null;
	}

	private static bool RequiresEmailLogin() {
		FirebaseSettings settings = GamePlayData.Instance != null
			? GamePlayData.Instance.FirebaseSettings
			: Resources.Load<FirebaseSettings>("Datas/FirebaseSettings");
		return settings != null && settings.RequireEmailLogin;
	}

	private Button EnsureLogoutButton() {
		if (_logoutButton != null)
			return _logoutButton;

		Canvas canvas = _loginButton != null ? _loginButton.GetComponentInParent<Canvas>() : null;
		if (canvas == null)
			return null;

		Transform existingButton = canvas.transform.Find(LogoutButtonName);
		if (existingButton != null && existingButton.TryGetComponent(out Button existing))
			return existing;

		GameObject buttonObject = new GameObject(LogoutButtonName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		buttonObject.layer = canvas.gameObject.layer;
		buttonObject.transform.SetParent(canvas.transform, false);
		buttonObject.transform.SetAsLastSibling();

		RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.pivot = Vector2.zero;
		rectTransform.anchoredPosition = new Vector2(24f, 24f);
		rectTransform.sizeDelta = new Vector2(112f, 36f);

		Image image = buttonObject.GetComponent<Image>();
		image.color = new Color(0.08f, 0.08f, 0.08f, 0.72f);

		Button button = buttonObject.GetComponent<Button>();
		ColorBlock colors = button.colors;
		colors.normalColor = image.color;
		colors.highlightedColor = new Color(0.15f, 0.15f, 0.15f, 0.86f);
		colors.pressedColor = new Color(0.04f, 0.04f, 0.04f, 0.9f);
		colors.selectedColor = colors.highlightedColor;
		colors.disabledColor = new Color(0.08f, 0.08f, 0.08f, 0.35f);
		button.colors = colors;

		GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
		textObject.layer = canvas.gameObject.layer;
		textObject.transform.SetParent(buttonObject.transform, false);

		RectTransform textRect = textObject.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = Vector2.zero;
		textRect.offsetMax = Vector2.zero;

		TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI sourceText = _loginButton != null ? _loginButton.GetComponentInChildren<TextMeshProUGUI>(true) : null;
		if (sourceText != null) {
			text.font = sourceText.font;
			text.fontSharedMaterial = sourceText.fontSharedMaterial;
		}

		text.text = "로그아웃";
		text.fontSize = 18f;
		text.color = Color.white;
		text.alignment = TextAlignmentOptions.Center;
		text.raycastTarget = false;

		buttonObject.SetActive(false);
		return button;
	}

	private void SetLogoutButtonVisible(bool visible) {
		if (_logoutButton == null)
			return;

		_logoutButton.gameObject.SetActive(visible);
		_logoutButton.interactable = visible;
	}

	private void UnsubscribeLoginPanelEvents() {
		if (_loginPanel == null)
			return;

		_loginPanel.OnEmailSignInClicked -= OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked -= OnEmailSignUpClicked;
	}

	private void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
