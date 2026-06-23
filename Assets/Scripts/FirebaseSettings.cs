using UnityEngine;

[CreateAssetMenu(fileName = "FirebaseSettings", menuName = "Firebase/Settings")]
public class FirebaseSettings : ScriptableObject {
	public enum VerificationType {
		Anonymous,
		Email
	}
	
	public bool UseFirebase;
	public VerificationType Type;
}