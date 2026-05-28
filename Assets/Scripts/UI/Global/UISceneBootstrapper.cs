using System.Collections;
using UnityEngine.SceneManagement;

public class UISceneBootstrapper : Singleton<UISceneBootstrapper> {
	private const string k_UISceneName = "UIScene";
	private string _currentMainScene = "MainScene";

	// UIScene은 Awake에서 자동 로드하지 않는다.
	// 메인 메뉴에서는 HUD가 필요 없으므로, 게임플레이 씬으로 전환할 때만 로드한다.
	protected override void Awake() {
		base.Awake();
	}

	public void TransitionTo(string sceneName) {
		StartCoroutine(DoTransition(sceneName));
	}

	private IEnumerator DoTransition(string sceneName) {
		if (_currentMainScene != null)
			yield return SceneManager.UnloadSceneAsync(_currentMainScene);

		// 메인 메뉴로 돌아갈 때는 UIScene 언로드, 게임플레이 씬으로 갈 때는 UIScene 로드
		bool needsUIScene = sceneName != "MainScene";
		bool uiSceneLoaded = SceneManager.GetSceneByName(k_UISceneName).isLoaded;

		if (needsUIScene && !uiSceneLoaded)
			yield return SceneManager.LoadSceneAsync(k_UISceneName, LoadSceneMode.Additive);
		else if (!needsUIScene && uiSceneLoaded)
			yield return SceneManager.UnloadSceneAsync(k_UISceneName);

		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		_currentMainScene = sceneName;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
	}
}
