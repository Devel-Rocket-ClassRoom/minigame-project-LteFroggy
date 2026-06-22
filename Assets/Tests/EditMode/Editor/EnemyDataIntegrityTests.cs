using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class EnemyDataIntegrityTests {
	[Test]
	public void AllEnemyDataAssetsHaveRequiredReferences() {
		EnemyData[] enemies = Resources.LoadAll<EnemyData>("Datas/Enemies/EnemyData");

		Assert.That(enemies, Is.Not.Empty);
		foreach (var enemy in enemies) {
			Assert.That(enemy.sprite, Is.Not.Null, $"{enemy.name} has no sprite.");
			Assert.That(enemy.animatorController, Is.Not.Null, $"{enemy.name} has no animator controller.");
			Assert.That(enemy.actions, Is.Not.Null, $"{enemy.name} has no action pattern list.");
			Assert.That(enemy.actions, Is.Not.Empty, $"{enemy.name} has no action pattern.");
			foreach (var pattern in enemy.actions) {
				Assert.That(pattern, Is.Not.Null, $"{enemy.name} has a null action pattern.");
				Assert.That(pattern.actions, Is.Not.Null, $"{pattern.name} has no actions list.");
				Assert.That(pattern.actions, Is.Not.Empty, $"{pattern.name} has no actions.");
				foreach (var action in pattern.actions) {
					Assert.That(action, Is.Not.Null, $"{pattern.name} has a null action.");
				}
			}
		}
	}

	[Test]
	public void AllEnemyDataAnimatorControllersMatchAllowedRules() {
		EnemyData[] enemies = Resources.LoadAll<EnemyData>("Datas/Enemies/EnemyData");

		Assert.That(enemies, Is.Not.Empty);
		foreach (var enemy in enemies) {
			bool hasValidationError = enemy.TryGetAnimatorControllerValidationError(out string validationError);
			Assert.That(hasValidationError, Is.False, validationError);
		}
	}

	[Test]
	public void GeneralEnemyDataRejectsStandaloneAnimatorController() {
		EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
		enemy.name = "Invalid General Enemy";
		enemy.animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Player/Player.controller");

		Assert.That(enemy.animatorController, Is.Not.Null);
		Assert.That(enemy.TryGetAnimatorControllerValidationError(out string validationError), Is.True);
		Assert.That(validationError, Does.Contain("AnimatorOverrideController"));
	}

	[Test]
	public void GeneralEnemyDataRejectsOverrideControllerWithoutEnemyBase() {
		RuntimeAnimatorController playerController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Player/Player.controller");
		Assert.That(playerController, Is.Not.Null);
		AnimatorOverrideController overrideController = new AnimatorOverrideController {
			runtimeAnimatorController = playerController
		};
		EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
		enemy.name = "Invalid Override Enemy";
		enemy.animatorController = overrideController;

		Assert.That(enemy.TryGetAnimatorControllerValidationError(out string validationError), Is.True);
		Assert.That(validationError, Does.Contain("EnemyBase.controller"));
	}

	[Test]
	public void FullAnimatorControllerExceptionDoesNotAllowInvalidOverrideController() {
		RuntimeAnimatorController playerController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Player/Player.controller");
		Assert.That(playerController, Is.Not.Null);
		AnimatorOverrideController overrideController = new AnimatorOverrideController {
			runtimeAnimatorController = playerController
		};
		EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
		enemy.name = "Invalid Override Boss";
		enemy.animatorController = overrideController;
		enemy.allowFullAnimatorController = true;

		Assert.That(enemy.TryGetAnimatorControllerValidationError(out string validationError), Is.True);
		Assert.That(validationError, Does.Contain("EnemyBase.controller"));
	}

	[Test]
	public void ExplicitFullAnimatorControllerExceptionAllowsBossController() {
		EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
		enemy.name = "Boss Enemy";
		enemy.animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Enemies/Boss/RitualBeast.controller");
		enemy.allowFullAnimatorController = true;

		Assert.That(enemy.animatorController, Is.Not.Null);
		bool hasValidationError = enemy.TryGetAnimatorControllerValidationError(out string validationError);
		Assert.That(hasValidationError, Is.False, validationError);
	}

	[Test]
	public void AllEnemySpawnTablesHaveEnemies() {
		EnemySpawnTable[] tables = Resources.LoadAll<EnemySpawnTable>("Datas/Enemies/EnemySpawnTable");

		Assert.That(tables, Is.Not.Empty);
		foreach (var table in tables) {
			Assert.That(table.enemyList, Is.Not.Null, $"{table.name} has no enemy list.");
			Assert.That(table.enemyList, Is.Not.Empty, $"{table.name} has no enemies.");
			foreach (var enemy in table.enemyList) {
				Assert.That(enemy, Is.Not.Null, $"{table.name} has a null enemy.");
			}
		}
	}
}
