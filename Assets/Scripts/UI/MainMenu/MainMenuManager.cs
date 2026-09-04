using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	private const string LogoutButtonName = "LogoutButton";
	private const string EmailLinkButtonName = "EmailLinkButton";
	private const string MetaShopButtonName = "MetaShopButton";

	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _newRunButton;
	[SerializeField] private Button _statsButton;
	[SerializeField] private Button _loginButton;
	[SerializeField] private Button _logoutButton;
	[SerializeField] private Button _emailLinkButton;
	[SerializeField] private Button _shopButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private GameObject _loadoutPanel;
	[SerializeField] private LoginManager _loginPanel;
	[SerializeField] private UserStatsPanelController _statsPanel;
	[SerializeField] private Image _loginStatusPanel;

	private bool _emailLinkMode;

	private async void OnEnable() {
		_logoutButton = EnsureLogoutButton();
		_emailLinkButton = EnsureEmailLinkButton();
		_shopButton = EnsureShopButton();

		_quitButton.onClick.AddListener(QuitGame);
		_newRunButton.onClick.AddListener(OnNewRunClicked);
		if (_statsButton != null)
			_statsButton.onClick.AddListener(OpenStats);
		_loginButton.onClick.AddListener(OpenLogin);
		if (_logoutButton != null)
			_logoutButton.onClick.AddListener(OnLogoutClicked);
		if (_emailLinkButton != null)
			_emailLinkButton.onClick.AddListener(OpenEmailLink);
		if (_shopButton != null)
			_shopButton.onClick.AddListener(OpenShop);

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
		if (_emailLinkButton != null)
			_emailLinkButton.onClick.RemoveListener(OpenEmailLink);
		if (_shopButton != null)
			_shopButton.onClick.RemoveListener(OpenShop);
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

				if (isLoggedIn)
					await LoadMetaProgressAsync();
				ShowStartButton(isLoggedIn);
				return;
			}

			if (bootstrapper.AuthManager != null && bootstrapper.AuthManager.IsLoggedIn) {
				await LoadMetaProgressAsync();
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

	private void OpenShop() {
		if (_loadoutPanel != null)
			_loadoutPanel.SetActive(false);
		if (_loginPanel != null)
			_loginPanel.gameObject.SetActive(false);
		if (_statsPanel != null)
			_statsPanel.Hide();

		LoadMetaProgressAsync().Forget();
		MetaShopPanelController.Show();
	}
	
	private void OpenLogin() {
		_emailLinkMode = false;
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
		_loginPanel.OnAnonymousSignInClicked += OnAnonymousSignInClicked;

		_loginPanel.gameObject.SetActive(true);
		_loginPanel.ConfigureForEmailAuth();
	}

	private void OpenEmailLink() {
		if (!IsAnonymous()) {
			SetEmailLinkButtonVisible(false);
			return;
		}

		_emailLinkMode = true;
		if (_loadoutPanel != null)
			_loadoutPanel.SetActive(false);
		if (_statsPanel != null)
			_statsPanel.Hide();
		if (_loginPanel == null) {
			Debug.LogError("[MainMenuManager] 로그인 패널이 연결되어 있지 않습니다.");
			return;
		}

		UnsubscribeLoginPanelEvents();
		_loginPanel.OnEmailSignInClicked += OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked += OnEmailSignUpClicked;
		_loginPanel.gameObject.SetActive(true);
		_loginPanel.ConfigureForEmailLink();
	}
	
	private async void OnEmailSignUpClicked(string email, string password) {
		var authManager = FirebaseBootstrapper.Instance.AuthManager;
		var (result, error) = _emailLinkMode
			? await authManager.LinkAnonymousWithEmail(email, password)
			: await authManager.SignUp(email, password);
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private async void OnEmailSignInClicked(string email, string password) {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignIn(email, password);
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
	}
	private async void OnAnonymousSignInClicked() {
		FirebaseAuthManager authManager = GetAuthManager();
		if (authManager == null) {
			_loginPanel.ShowError("Firebase 인증이 준비되지 않았습니다.");
			return;
		}

		var (result, error) = await authManager.SignIn();
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
	}

	
	private void LoginSuccess() {
		_emailLinkMode = false;
		UnsubscribeLoginPanelEvents();
		if (_loginPanel != null)
			_loginPanel.gameObject.SetActive(false);

		LoadMetaProgressAsync().Forget();
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
		SetEmailLinkButtonVisible(false);
		SetShopButtonVisible(false);
		HideLoginStatus();
	}

	private void ShowStartButton(bool interactable = true) {
		_loginButton.gameObject.SetActive(false);
		_loginButton.interactable = false;

		_newRunButton.interactable = interactable;
		_newRunButton.gameObject.SetActive(true);
		SetStatsButton(interactable);

		SetLogoutButtonVisible(interactable && RequiresEmailLogin() && IsLoggedIn());
		SetEmailLinkButtonVisible(interactable && IsAnonymous());
		SetShopButtonVisible(interactable);
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

	private bool IsAnonymous() {
		FirebaseAuthManager authManager = GetAuthManager();
		return authManager != null && authManager.IsAnonymous;
	}

	private async UniTask LoadMetaProgressAsync() {
		FirebaseMetaProgressManager manager = FirebaseBootstrapper.Instance != null
			? FirebaseBootstrapper.Instance.MetaProgressManager
			: null;
		if (manager == null)
			return;

		var (success, error) = await manager.LoadOrCreateMetaProgress();
		if (!success)
			Debug.LogWarning($"[MainMenuManager] 메타 진행 로드 실패: {error}");
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

	private Button EnsureEmailLinkButton() {
		return EnsureUtilityButton(EmailLinkButtonName, "이메일 연결", new Vector2(24f, 68f), new Vector2(138f, 36f));
	}

	private Button EnsureShopButton() {
		return EnsureUtilityButton(MetaShopButtonName, "상점", new Vector2(24f, 112f), new Vector2(112f, 36f));
	}

	private Button EnsureUtilityButton(string objectName, string text, Vector2 anchoredPosition, Vector2 size) {
		Canvas canvas = _loginButton != null ? _loginButton.GetComponentInParent<Canvas>() : null;
		if (canvas == null)
			return null;

		Transform existingButton = canvas.transform.Find(objectName);
		if (existingButton != null && existingButton.TryGetComponent(out Button existing))
			return existing;

		GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
		buttonObject.layer = canvas.gameObject.layer;
		buttonObject.transform.SetParent(canvas.transform, false);
		buttonObject.transform.SetAsLastSibling();

		RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.pivot = Vector2.zero;
		rectTransform.anchoredPosition = anchoredPosition;
		rectTransform.sizeDelta = size;

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

		TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI sourceText = _loginButton != null ? _loginButton.GetComponentInChildren<TextMeshProUGUI>(true) : null;
		if (sourceText != null) {
			label.font = sourceText.font;
			label.fontSharedMaterial = sourceText.fontSharedMaterial;
		}

		label.text = text;
		label.fontSize = 18f;
		label.color = Color.white;
		label.alignment = TextAlignmentOptions.Center;
		label.raycastTarget = false;

		buttonObject.SetActive(false);
		return button;
	}

	private void SetLogoutButtonVisible(bool visible) {
		if (_logoutButton == null)
			return;

		_logoutButton.gameObject.SetActive(visible);
		_logoutButton.interactable = visible;
	}

	private void SetEmailLinkButtonVisible(bool visible) {
		if (_emailLinkButton == null)
			return;

		_emailLinkButton.gameObject.SetActive(visible);
		_emailLinkButton.interactable = visible;
	}

	private void SetShopButtonVisible(bool visible) {
		if (_shopButton == null)
			return;

		_shopButton.gameObject.SetActive(visible);
		_shopButton.interactable = visible;
	}

	private void UnsubscribeLoginPanelEvents() {
		if (_loginPanel == null)
			return;

		_loginPanel.OnEmailSignInClicked -= OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked -= OnEmailSignUpClicked;
		_loginPanel.OnAnonymousSignInClicked -= OnAnonymousSignInClicked;
	}

	private void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
