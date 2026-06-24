using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour, IEmailAuthManager, IAnonymousAuthManager {
	private const string LogPrefix = "[" + nameof(FirebaseBootstrapper) + "]";
	
	private FirebaseAuth _auth;
	private FirebaseUser _currentUser;
	
	public bool IsLoggedIn => _currentUser != null;
	public string UserId => _currentUser?.UserId;
	
	public void Initialize(FirebaseAuth auth) {
		_auth = auth;
		_currentUser = _auth.CurrentUser;
		Debug.Log($"{(_currentUser != null ? $"{LogPrefix} 이미 로그인된 상태입니다." : $"{LogPrefix} 로그인이 필요합니다")}");
		NotifyLoginState();
	}

	public async UniTask<(bool success, string error)> SignUp(string email, string password) {
		try {
			Debug.Log($"{LogPrefix} 이메일 회원가입 시도");
	
			AuthResult result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
			_currentUser = result.User;
			NotifyLoginState();

			Debug.Log($"{LogPrefix} 이메일 회원가입 성공 {_currentUser.UserId}");
	
			return (true, null);	
		} catch (Exception e) {
			Debug.Log($"{LogPrefix} 이메일 회원가입 실패 : {e.Message}");
			return (false, ParseFirebaseError(e.Message));
		}
	}
	
	public async UniTask<(bool success, string error)> SignIn(string email, string password) {
		try {
			Debug.Log($"{LogPrefix} 이메일 로그인 시도");
		
			AuthResult result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
			_currentUser = result.User;
			NotifyLoginState();

			Debug.Log($"{LogPrefix} 이메일 로그인 성공 {_currentUser.UserId}");
		
			return (true, null);	
		} catch (Exception e) {
			Debug.Log($"{LogPrefix} 이메일 로그인 실패 : {e.Message}");
			return (false, ParseFirebaseError(e.Message));
		}
	}
	
	public async UniTask<(bool success, string error)> SignIn() {
		try {
			Debug.Log($"{LogPrefix} 익명 로그인 시도 ...");
		    
			AuthResult result = await _auth.SignInAnonymouslyAsync();
			_currentUser = result.User;
			NotifyLoginState();
		    
			Debug.Log($"{LogPrefix} 익명 로그인 성공 {_currentUser.UserId}");
		    
			return (true, null);
		} catch (Exception e) {
			Debug.Log($"{LogPrefix} 익명 로그인 실패 {e.Message}");
			return (false, ParseFirebaseError(e.Message));
		}
	}

	public (bool success, string error) SignOut() {
		if (_auth == null) {
			_currentUser = null;
			NotifyLoginState();
			return (false, "Firebase 인증이 준비되지 않았습니다.");
		}

		try {
			Debug.Log($"{LogPrefix} 로그아웃 시도");
			_auth.SignOut();
			_currentUser = _auth.CurrentUser;
			NotifyLoginState();
			return (_currentUser == null, _currentUser == null ? null : "로그아웃 처리 후에도 로그인 세션이 남아 있습니다.");
		}
		catch (Exception e) {
			Debug.LogWarning($"{LogPrefix} 로그아웃 실패 : {e.Message}");
			_currentUser = _auth.CurrentUser;
			NotifyLoginState();
			return (false, "로그아웃 처리에 실패했습니다.");
		}
	}

	private void Start() {
		if (_auth == null)
			return;

		_currentUser = _auth.CurrentUser;
		Debug.Log($"{(_currentUser != null ? $"{LogPrefix} 이미 로그인된 상태입니다." : $"{LogPrefix} 로그인이 필요합니다")}");
		NotifyLoginState();
	}
	
	private void NotifyLoginState() {
		Debug.Log(_currentUser != null ? $"{LogPrefix} 로그인 상태 : {_currentUser.UserId}" : $"{LogPrefix} 로그아웃 상태");
	}
	
	private string ParseFirebaseError(string error)
	{
		Debug.LogWarning($"[Auth] Firebase 에러 원문: {error}");

		string lower = error.ToLowerInvariant();

		if (lower.Contains("already in use") || lower.Contains("email-already"))
		{
			return "이미 사용 중인 이메일입니다.";
		}
		if (lower.Contains("at least 6") || lower.Contains("weak") || lower.Contains("password is invalid"))
		{
			return "비밀번호는 6자 이상이어야 합니다.";
		}
		if (lower.Contains("badly formatted") || lower.Contains("invalid-email"))
		{
			return "이메일 형식이 올바르지 않습니다.";
		}
		if (lower.Contains("network"))
		{
			return "네트워크 연결을 확인해주세요.";
		}

		return "이메일 또는 비밀번호를 확인해주세요.";
	}
}
