using Cysharp.Threading.Tasks;

public interface IAnonymousAuthManager : IAuthManager {
	public UniTask<(bool success, string error)> SignIn();
}