using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	private static T _instance;
	public static T Instance {
		get {
			_instance = FindAnyObjectByType<T>();
			if (_instance == null) { _instance = new GameObject(typeof(T).Name).AddComponent<T>(); }
			return _instance;
		}
	}
	
	protected virtual void Awake() {
		// Instance가 이미 있는데, 내가 아니라면 나는 할복
		if (_instance != null && _instance != this) {
			Destroy(gameObject);
			return;
		}
		_instance = (T)this;
		DontDestroyOnLoad(gameObject);
	}
	
	protected virtual void OnDestroy() {
		_instance = null;
	}
}