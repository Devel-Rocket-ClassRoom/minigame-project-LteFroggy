using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _newRunButton;
	[SerializeField] private Button _loginButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private GameObject _loadoutPanel;
	[SerializeField] private LoginManager _loginPanel;
	[SerializeField] private Image _loginStatusPanel;

	private async void OnEnable() {
		_quitButton.onClick.AddListener(QuitGame);
		_newRunButton.onClick.AddListener(OpenLoadout);
		_loginButton.onClick.AddListener(OpenLogin);

		ShowLoginButton(false);

		PrepareButtonAsync().Forget();
	}

	private void OnDisable() {
		_newRunButton.onClick.RemoveListener(OpenLoadout);
		_loginButton.onClick.RemoveListener(OpenLogin);
		_quitButton.onClick.RemoveListener(QuitGame);
		UnsubscribeLoginPanelEvents();
	}
	
	private async UniTaskVoid PrepareButtonAsync() {
		// Firebase 사용 시, 로딩 후 로그인 버튼 켜주기
		await UniTask.WaitUntil(() => FirebaseBootstrapper.Instance != null);

		FirebaseBootstrapper bootstrapper = FirebaseBootstrapper.Instance;
		InitState state = await bootstrapper.WaitForInitializationAsync();

		if (state == InitState.Ready) {
			if (bootstrapper.AuthManager != null && bootstrapper.AuthManager.IsLoggedIn) {
				ShowStartButton("로그인됨");
				return;
			}

			ShowLoginButton(true);
			return;
		}

		ShowStartButton(string.Empty);
	}
	
	private void OpenLoadout() {
		_loadoutPanel.SetActive(true);
		if (_loadoutPanel.TryGetComponent(out LoadoutManager loadoutManager))
			loadoutManager.RefreshRelicList();
	}
	
	private void OpenLogin() {
		// 열릴 때, 이미 로그인 되어이쓴 상태라면 패널 열 필요 없이 바로 로드아웃으로
		if (FirebaseBootstrapper.Instance.AuthManager.IsLoggedIn) {
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
		_loginPanel.OnAnonymousSignInClicked += OnAnonymousSignInClicked;
		_loginPanel.OnEmailSignInClicked += OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked += OnEmailSignUpClicked;

		_loginPanel.gameObject.SetActive(true);
	}
	
	private async void OnAnonymousSignInClicked() {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignIn();
		if (result) { LoginSuccess(); }
		else { _loginPanel.ShowError(error); }
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

		ShowStartButton("로그인됨");
	}

	private void ShowLoginButton(bool interactable) {
		_newRunButton.interactable = false;
		_newRunButton.gameObject.SetActive(false);

		_loginButton.interactable = interactable;
		_loginButton.gameObject.SetActive(true);

		SetLoginStatus(string.Empty);
	}

	private void ShowStartButton(string statusText) {
		_loginButton.gameObject.SetActive(false);
		_loginButton.interactable = false;

		_newRunButton.interactable = true;
		_newRunButton.gameObject.SetActive(true);

		SetLoginStatus(statusText);
	}

	private void SetLoginStatus(string statusText) {
		if (_loginStatusPanel == null)
			return;

		_loginStatusPanel.gameObject.SetActive(!string.IsNullOrEmpty(statusText));
	}

	private void UnsubscribeLoginPanelEvents() {
		if (_loginPanel == null)
			return;

		_loginPanel.OnAnonymousSignInClicked -= OnAnonymousSignInClicked;
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
