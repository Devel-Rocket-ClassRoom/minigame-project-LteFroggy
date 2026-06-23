using Cysharp.Threading.Tasks;
using Firebase.Auth;

public interface IEmailAuthManager : IAuthManager {
	public UniTask<(bool success, string error)> SignIn(string email, string password);
	public UniTask<(bool success, string error)> SignUp(string email, string password);
}