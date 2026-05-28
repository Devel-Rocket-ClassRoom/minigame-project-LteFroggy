using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
		bool needsUIScene = sceneName != "MainScene";
		bool uiSceneLoaded = SceneManager.GetSceneByName(k_UISceneName).isLoaded;

		DeactivateSceneAudioAndInput(_currentMainScene);

		if (needsUIScene && !uiSceneLoaded)
			yield return SceneManager.LoadSceneAsync(k_UISceneName, LoadSceneMode.Additive);

		if (uiSceneLoaded && _currentMainScene != null) {
			// UIScene이 이미 로드된 상태라면 씬이 0개가 될 걱정 없이 이전 씬 먼저 언로드
			yield return SceneManager.UnloadSceneAsync(_currentMainScene);
			yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		} else {
			// UIScene이 없을 때(MainScene에서 첫 전환)는 새 씬 먼저 로드해야 마지막 씬 언로드 오류 방지
			yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			if (_currentMainScene != null)
				yield return SceneManager.UnloadSceneAsync(_currentMainScene);
		}

		if (!needsUIScene && uiSceneLoaded)
			yield return SceneManager.UnloadSceneAsync(k_UISceneName);

		_currentMainScene = sceneName;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
	}

	// 씬 전환 중 EventSystem·AudioListener 중복을 막기 위해 해당 씬의 컴포넌트를 비활성화한다.
	// 씬 자체가 곧 언로드되므로 원복은 불필요하다.
	private static void DeactivateSceneAudioAndInput(string sceneName) {
		if (sceneName == null) return;
		var scene = SceneManager.GetSceneByName(sceneName);
		if (!scene.IsValid() || !scene.isLoaded) return;

		foreach (var root in scene.GetRootGameObjects()) {
			foreach (var es in root.GetComponentsInChildren<EventSystem>(true))
				es.gameObject.SetActive(false);
			foreach (var al in root.GetComponentsInChildren<AudioListener>(true))
				al.enabled = false;
		}
	}
}
