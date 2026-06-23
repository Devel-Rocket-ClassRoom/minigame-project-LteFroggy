using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class FirebaseRunResultSaveManager : MonoBehaviour {
	private FirebaseAuthManager _authManager;
	private FirebaseDatabase _database;

	public void Initialize(FirebaseAuthManager authManager, FirebaseDatabase database) {
		_authManager = authManager;
		_database = database;
	}

	public async UniTask<(bool success, string error)> SaveRunData(RunResult result, BattleManager battleManager = null) {
		if (_database == null)
			return (false, "[FirebaseRunResultSaveManager] Firebase Database가 초기화되지 않았습니다.");

		if (_authManager == null || !_authManager.IsLoggedIn)
			return (false, "[FirebaseRunResultSaveManager] 로그인 정보가 없습니다.");

		try {
			Debug.Log($"[FirebaseRunResultSaveManager] 런 결과 저장 시도");
			RunResultData runResult = BuildRunResultData(result, battleManager);
			string json = JsonUtility.ToJson(runResult);

			await _database.RootReference
				.Child("users")
				.Child(_authManager.UserId)
				.Child("runResults")
				.Push()
				.SetRawJsonValueAsync(json)
				.AsUniTask();

			
			Debug.Log($"[FirebaseRunResultSaveManager] 런 결과 저장 성공");
			return (true, null);
		}
		catch (Exception e) {
			Debug.LogError($"[FirebaseRunResultSaveManager] 런 결과 저장 실패: {e.Message}");
			return (false, e.Message);
		}
	}

	private RunResultData BuildRunResultData(RunResult result, BattleManager battleManager) {
		GamePlayData data = GamePlayData.Instance;
		EnemySpawnTable enemySpawnTable = battleManager != null
			? battleManager.EnemyManager.CurrentSpawnTable
			: null;

		return new RunResultData {
			result = result.ToString(),
			enemyEncounterId = GetEnemyEncounterId(enemySpawnTable),
			enemyIds = enemySpawnTable != null && enemySpawnTable.enemyList != null
				? enemySpawnTable.enemyList
					.Where(enemy => enemy != null)
					.Select(enemy => enemy.id)
					.ToArray()
				: Array.Empty<int>(),
			currentHealth = data.CurrentHealth,
			maxHealth = data.MaxHealth,
			gold = data.Gold,
			deckCount = data.Deck.Count,
			relicCount = data.Relics.Count,
			cardIds = data.Deck
				.Select(card => card._cardDefinition.cardId.ToString())
				.ToArray(),
			relicIds = data.Relics
				.Select(relic => relic.GetType().Name)
				.ToArray(),
			savedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
		};
	}

	private static string GetEnemyEncounterId(EnemySpawnTable enemySpawnTable) {
		if (enemySpawnTable == null)
			return string.Empty;

		return string.IsNullOrWhiteSpace(enemySpawnTable.encounterId)
			? enemySpawnTable.name
			: enemySpawnTable.encounterId;
	}
}
