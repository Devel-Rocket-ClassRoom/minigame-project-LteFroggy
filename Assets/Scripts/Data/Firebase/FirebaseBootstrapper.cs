using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

// Firebase 초기화를 프로젝트에서 한 번만 담당하는 부트스트래퍼입니다.
// 다른 스크립트는 Firebase를 직접 초기화하지 말고,
// 이 클래스가 준비한 App/Auth/Database를 기다렸다가 사용합니다.
public class FirebaseBootstrapper : MonoBehaviour {
	private const string LogPrefix = "[" + nameof(FirebaseBootstrapper) + "]";

	public static FirebaseBootstrapper Instance { get; private set; }

	public InitState State { get; private set; } = InitState.Pending;
	public bool IsReady => State == InitState.Ready;
	public string LastError { get; private set; }

	// Firebase 기본 앱과 자주 쓰는 서비스 참조입니다.
	public FirebaseApp App { get; private set; }
	public FirebaseAuthManager AuthManager { get; private set; }
	public FirebaseRunResultManager RunResultManager { get; private set; }
	public FirebaseMetaProgressManager MetaProgressManager { get; private set; }
	public FirebaseRunSnapshotManager RunSnapshotManager { get; private set; }

	private void Awake() {
		// 씬 전환이나 중복 배치로 Bootstrapper가 여러 개 생기는 것을 막습니다.
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		// Firebase 초기화는 여기서 한 번만 시작합니다.
		InitializeAsync().Forget();
	}

	private async UniTaskVoid InitializeAsync() {
		Debug.Log($"{LogPrefix} 초기화 시작");

		try {
			FirebaseSettings settings = GamePlayData.Instance != null ? GamePlayData.Instance.FirebaseSettings : null;

			// 세팅을 넣지 않았다면 Firebase를 초기화하지 않습니다.
			if (settings == null) {
				DisableFirebase();
				return;
			}
			
			// 현재 플랫폼에서 Firebase 의존성이 사용 가능한지 확인합니다.
			Debug.Log($"{LogPrefix} 의존성 확인 시작");
			DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
			if (status != DependencyStatus.Available) {
				Fail($"{LogPrefix} 의존성 오류: {status}");
				return;
			}
			Debug.Log($"{LogPrefix} 의존성 확인 완료");

			// google-services 설정 파일을 기반으로 기본 Firebase 인스턴스를 가져옵니다.
			App = FirebaseApp.DefaultInstance;
			
			// 완료하면, 붙여주기
			Debug.Log($"{LogPrefix} Auth 서비스 초기화 시작");
			AuthManager = gameObject.AddComponent<FirebaseAuthManager>();
			AuthManager.Initialize(FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance));
			Debug.Log($"{LogPrefix} Auth 서비스 초기화 완료");
			
			Debug.Log($"{LogPrefix} 런 결과 서비스 초기화 시작");
			RunResultManager = gameObject.AddComponent<FirebaseRunResultManager>();
			RunResultManager.Initialize(AuthManager, FirebaseDatabase.GetInstance(App));
			Debug.Log($"{LogPrefix} 런 결과 서비스 초기화 완료");

			Debug.Log($"{LogPrefix} 메타 진행 서비스 초기화 시작");
			MetaProgressManager = gameObject.AddComponent<FirebaseMetaProgressManager>();
			MetaProgressManager.Initialize(AuthManager, FirebaseDatabase.GetInstance(App));
			Debug.Log($"{LogPrefix} 메타 진행 서비스 초기화 완료");

			Debug.Log($"{LogPrefix} 현재 런 스냅샷 서비스 초기화 시작");
			RunSnapshotManager = gameObject.AddComponent<FirebaseRunSnapshotManager>();
			RunSnapshotManager.Initialize(AuthManager, FirebaseDatabase.GetInstance(App));
			Debug.Log($"{LogPrefix} 현재 런 스냅샷 서비스 초기화 완료");

			State = InitState.Ready;
			Debug.Log($"{LogPrefix} 초기화 완료: {App.Name}");
			
			var app = FirebaseApp.DefaultInstance;
			var options = app.Options;
			
			Debug.Log($"Firebase ProjectId : {options.ProjectId}");
		}
		catch (Exception ex) {
			Fail(ex.Message);
		}
	}

	// Firebase 초기화가 끝날 때까지 기다리는 함수입니다.
	// Pending이면 대기하고, 아니라면 바로 결과를 반환합니다.
	// 이 함수는 초기화를 다시 시도하지 않습니다.
	public async UniTask<InitState> WaitForInitializationAsync() {
		await UniTask.WaitUntil(() => State != InitState.Pending);
		return State;
	}

	private void Fail(string error) {
		LastError = error;
		State = InitState.Failed;
		Debug.LogError($"{LogPrefix} 초기화 실패 : {LastError}");
	}
	
	private void DisableFirebase() {
		LastError = $"Firebase 사용하지 않음";
		State = InitState.Disabled;
		Debug.Log($"{LogPrefix} 초기화 하지 않음 : {LastError}");
	}

	private void OnDestroy() {
		if (Instance == this) {
			Instance = null;
		}
	}
}
