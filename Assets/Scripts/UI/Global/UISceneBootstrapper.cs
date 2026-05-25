using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneBootstrapper : Singleton<UISceneBootstrapper> {
	private const string k_UISceneName = "UIScene";

	protected override void Awake() {
		base.Awake();
		if (!SceneManager.GetSceneByName(k_UISceneName).isLoaded) {
			SceneManager.LoadSceneAsync(k_UISceneName, LoadSceneMode.Additive);
		}
	}
}
