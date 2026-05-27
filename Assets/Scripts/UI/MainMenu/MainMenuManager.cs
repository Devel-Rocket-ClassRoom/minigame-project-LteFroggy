using UnityEngine;
using UnityEngine.UI;

// 메인 화면 관리자
// 역할: 새 런 시작 버튼을 누르면 로드아웃 패널을 열어준다
public class MainMenuManager : MonoBehaviour {
	[SerializeField] private Button _newRunButton;
	[SerializeField] private GameObject _loadoutPanel;

	private void OnEnable() {
		_newRunButton.onClick.AddListener(OpenLoadout);
	}

	private void OnDisable() {
		_newRunButton.onClick.RemoveListener(OpenLoadout);
	}

	private void OpenLoadout() {
		_loadoutPanel.SetActive(true);
	}
}
