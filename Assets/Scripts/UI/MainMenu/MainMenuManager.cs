using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _newRunButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private GameObject _loadoutPanel;
	[SerializeField] private LoginManager _loginPanel;

	private async void OnEnable() {
		_newRunButton.interactable = false;
		_quitButton.onClick.AddListener(QuitGame);

		PrepareNewRunButtonAsync().Forget();
	}

	private void OnDisable() {
		_newRunButton.onClick.RemoveAllListeners();
		_quitButton.onClick.RemoveListener(QuitGame);
	}
	
	private async UniTaskVoid PrepareNewRunButtonAsync() {
		await UniTask.WaitUntil(() => FirebaseBootstrapper.Instance != null);

		FirebaseBootstrapper bootstrapper = FirebaseBootstrapper.Instance;
		InitState state = await bootstrapper.WaitForInitializationAsync();

		_newRunButton.onClick.RemoveAllListeners();

		if (state == InitState.Ready) {
			_newRunButton.onClick.AddListener(OpenLogin);
		} else {
			_newRunButton.onClick.AddListener(OpenLoadout);
		}

		_newRunButton.interactable = true;
	}
	
	private void OpenLoadout() {
		CloseLoginPanel();
		_loadoutPanel.SetActive(true);
		if (_loadoutPanel.TryGetComponent(out LoadoutManager loadoutManager))
			loadoutManager.RefreshRelicList();
	}
	
	private void OpenLogin() {
		// 열릴 때, 이미 로그인 되어이쓴 상태라면 패널 열 필요 없이 바로 로드아웃으로
		if (FirebaseBootstrapper.Instance.AuthManager.IsLoggedIn) {
			Debug.Log("[MainMenuManager] 이미 로그인되어있어 바로 로드아웃 패널로 연결됩니다.");
			OpenLoadout();
			return;
		}
		
		_loadoutPanel.SetActive(false);
		if (_loginPanel == null) {
			Debug.LogError("[MainMenuManager] 로그인 패널이 연결되어 있지 않습니다.");
			return;
		}
		
		_loginPanel.OnAnonymousSignInClicked += OnAnonymousSignInClicked;
		_loginPanel.OnEmailSignInClicked += OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked += OnEmailSignUpClicked;
		
		_loginPanel.gameObject.SetActive(true);
	}
	
	private async void OnAnonymousSignInClicked() {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignIn();
		if (result) { OpenLoadout(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private async void OnEmailSignUpClicked(string email, string password) {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignUp(email, password);
		if (result) { OpenLoadout(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private async void OnEmailSignInClicked(string email, string password) {
		var (result, error) = await FirebaseBootstrapper.Instance.AuthManager.SignIn(email, password);
		if (result) { OpenLoadout(); }
		else { _loginPanel.ShowError(error); }
	}
	
	private void CloseLoginPanel() {
		_loginPanel.OnAnonymousSignInClicked -= OnAnonymousSignInClicked;
		_loginPanel.OnEmailSignInClicked -= OnEmailSignInClicked;
		_loginPanel.OnEmailSignUpClicked -= OnEmailSignUpClicked;
		
		_loginPanel.gameObject.SetActive(false);
	}
	

	private void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
