using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
#if UNITY_EDITOR
	private const string EnemyBaseControllerPath = "Assets/Animations/Enemies/EnemyBase.controller";
#endif

	public int id;
	public int health;
	public string name;
	public Sprite sprite;
	public RuntimeAnimatorController animatorController;
	public bool allowFullAnimatorController;
	public List<EnemyActionPattern> actions = new();

#if UNITY_EDITOR
	private void OnValidate() {
		if (TryGetAnimatorControllerValidationError(out string validationError))
			Debug.LogWarning(validationError, this);
	}

	public bool TryGetAnimatorControllerValidationError(out string validationError) {
		if (animatorController == null) {
			validationError = $"{name} has no animator controller.";
			return true;
		}

		if (animatorController is not AnimatorOverrideController overrideController)
			return ValidateFullAnimatorController(out validationError);

		if (overrideController.runtimeAnimatorController == null) {
			validationError = $"{name} has an AnimatorOverrideController with no base controller.";
			return true;
		}

		string baseControllerPath = AssetDatabase.GetAssetPath(overrideController.runtimeAnimatorController);
		if (baseControllerPath != EnemyBaseControllerPath) {
			validationError = $"{name} AnimatorOverrideController must be based on {EnemyBaseControllerPath}, but was based on {baseControllerPath}.";
			return true;
		}

		validationError = null;
		return false;
	}

	private bool ValidateFullAnimatorController(out string validationError) {
		if (allowFullAnimatorController) {
			validationError = null;
			return false;
		}

		validationError = $"{name} must use an AnimatorOverrideController based on {EnemyBaseControllerPath}.";
		return true;
	}
#endif
}
