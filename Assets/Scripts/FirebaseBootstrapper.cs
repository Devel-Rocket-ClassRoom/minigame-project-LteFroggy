using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

// Firebase 초기화를 프로젝트에서 한 번만 담당하는 부트스트래퍼입니다.
// 다른 스크립트는 Firebase를 직접 초기화하지 말고,
// 이 클래스가 준비한 App/Auth/Database를 기다렸다가 사용합니다.
public class FirebaseBootstrapper : MonoBehaviour {
	public static FirebaseBootstrapper Instance { get; private set; }

	// Firebase 초기화 진행 상태입니다.
	public enum InitState {
		// 초기화가 아직 끝나지 않은 상태입니다.
		Pending,
		// 초기화가 성공해서 Firebase 기능을 사용할 수 있는 상태입니다.
		Ready,
		// 초기화가 실패해서 Firebase 기능을 사용할 수 없는 상태입니다.
		Failed
	}

	public InitState State { get; private set; } = InitState.Pending;
	public bool IsReady => State == InitState.Ready;
	public string LastError { get; private set; }

	// Firebase 기본 앱과 자주 쓰는 서비스 참조입니다.
	public FirebaseApp App { get; private set; }
	public FirebaseAuth Auth { get; private set; }
	public FirebaseDatabase Database { get; private set; }

	private void Awake() {
		// 씬 전환이나 중복 배치로 Bootstrapper가 여러 개 생기는 것을 막습니다.
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		// Firebase 초기화는 여기서 한 번만 시작합니다.
		// WaitForInitializationAsync는 이 작업이 끝나길 기다릴 뿐,
		// 초기화를 다시 실행하지 않습니다.
		InitializeAsync().Forget();
	}

	private async UniTaskVoid InitializeAsync() {
		Debug.Log("[FirebaseBootstrapper] 초기화 시작");

		try {
			// 현재 플랫폼에서 Firebase 의존성이 사용 가능한지 확인합니다.
			DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
			if (status != DependencyStatus.Available) {
				Fail($"[FirebaseBootstrapper] 의존성 오류: {status}");
				return;
			}

			// google-services 설정 파일을 기반으로 기본 Firebase 인스턴스를 가져옵니다.
			App = FirebaseApp.DefaultInstance;
			Auth = FirebaseAuth.GetAuth(App);
			Database = FirebaseDatabase.GetInstance(App);

			State = InitState.Ready;
			Debug.Log($"[FirebaseBootstrapper] 초기화 완료: {App.Name}");
		}
		catch (System.Exception ex) {
			Fail(ex.Message);
		}
	}

	// Firebase 초기화가 끝날 때까지 기다리는 함수입니다.
	// Pending이면 대기하고, 이미 Ready 또는 Failed라면 바로 결과를 반환합니다.
	// 이 함수는 초기화를 다시 시도하지 않습니다.
	public async UniTask<bool> WaitForInitializationAsync() {
		await UniTask.WaitUntil(() => State != InitState.Pending);
		return State == InitState.Ready;
	}

	private void Fail(string error) {
		LastError = error;
		State = InitState.Failed;
		Debug.LogError($"[FirebaseBootstrapper] 초기화 실패: {error}");
	}

	private void OnDestroy() {
		if (Instance == this) {
			Instance = null;
		}
	}
}
