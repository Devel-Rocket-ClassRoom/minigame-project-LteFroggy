using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _newRunButton;
	[SerializeField] private Button _quitButton;
	[SerializeField] private GameObject _loadoutPanel;

	private void OnEnable() {
		_newRunButton.onClick.AddListener(OpenLoadout);
		_quitButton.onClick.AddListener(QuitGame);
	}

	private void OnDisable() {
		_newRunButton.onClick.RemoveListener(OpenLoadout);
		_quitButton.onClick.RemoveListener(QuitGame);
	}

	private void OpenLoadout() {
		_loadoutPanel.SetActive(true);
		if (_loadoutPanel.TryGetComponent(out LoadoutManager loadoutManager))
			loadoutManager.RefreshRelicList();
	}

	private void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
