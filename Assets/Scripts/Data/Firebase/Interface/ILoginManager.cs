using System;

public interface ILoginManager {
	public event Action<string, string> OnEmailSignInClicked;
	public event Action<string, string> OnEmailSignUpClicked;
	public event Action OnAnonymousSignInClicked;

	public void ShowInfo(string message);
	public void ShowError(string message);
	public void ClearStatus();
}
