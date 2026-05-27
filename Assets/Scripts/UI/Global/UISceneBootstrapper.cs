using System.Collections;
using UnityEngine.SceneManagement;

public class UISceneBootstrapper : Singleton<UISceneBootstrapper> {
	private const string k_UISceneName = "UIScene";
	private string _currentMainScene = "StartScene";

	protected override void Awake() {
		base.Awake();
		if (!SceneManager.GetSceneByName(k_UISceneName).isLoaded) {
			SceneManager.LoadSceneAsync(k_UISceneName, LoadSceneMode.Additive);
		}
	}

	public void TransitionTo(string sceneName) {
		StartCoroutine(DoTransition(sceneName));
	}

	private IEnumerator DoTransition(string sceneName) {
		if (_currentMainScene != null)
			yield return SceneManager.UnloadSceneAsync(_currentMainScene);

		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		_currentMainScene = sceneName;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
	}
}
