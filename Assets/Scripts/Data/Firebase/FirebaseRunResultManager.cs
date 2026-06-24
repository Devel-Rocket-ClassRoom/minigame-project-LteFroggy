using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class FirebaseRunResultManager : MonoBehaviour {
	private FirebaseAuthManager _authManager;
	private FirebaseDatabase _database;

	public void Initialize(FirebaseAuthManager authManager, FirebaseDatabase database) {
		_authManager = authManager;
		_database = database;
	}

	public async UniTask<(bool success, string error)> SaveRunData(RunResult result, BattleManager battleManager = null) {
		if (_database == null)
			return (false, "[FirebaseRunResultManager] Firebase Database가 초기화되지 않았습니다.");

		if (_authManager == null || !_authManager.IsLoggedIn)
			return (false, "[FirebaseRunResultManager] 로그인 정보가 없습니다.");

		try {
			Debug.Log("[FirebaseRunResultManager] 런 결과 저장 시도");
			RunResultData runResult = BuildRunResultData(result, battleManager);
			string json = JsonUtility.ToJson(runResult);

			await _database.RootReference
				.Child("users")
				.Child(_authManager.UserId)
				.Child("runResults")
				.Push()
				.SetRawJsonValueAsync(json)
				.AsUniTask();

			Debug.Log("[FirebaseRunResultManager] 런 결과 저장 성공");
			return (true, null);
		}
		catch (Exception e) {
			Debug.LogError($"[FirebaseRunResultManager] 런 결과 저장 실패: {e.Message}");
			return (false, e.Message);
		}
	}

	public async UniTask<(bool success, string error, List<RunResultData> results)> LoadRunResults() {
		if (_database == null)
			return (false, "[FirebaseRunResultManager] Firebase Database가 초기화되지 않았습니다.", null);

		if (_authManager == null || !_authManager.IsLoggedIn)
			return (false, "[FirebaseRunResultManager] 로그인 정보가 없습니다.", null);

		try {
			Debug.Log("[FirebaseRunResultManager] 런 결과 조회 시도");

			DataSnapshot snapshot = await _database.RootReference
				.Child("users")
				.Child(_authManager.UserId)
				.Child("runResults")
				.GetValueAsync()
				.AsUniTask();

			var results = new List<RunResultData>();
			foreach (DataSnapshot child in snapshot.Children) {
				string json = child.GetRawJsonValue();
				if (string.IsNullOrWhiteSpace(json))
					continue;

				RunResultData runResult = JsonUtility.FromJson<RunResultData>(json);
				if (runResult != null)
					results.Add(runResult);
			}

			results.Sort((left, right) => right.savedAtUnixTime.CompareTo(left.savedAtUnixTime));
			Debug.Log($"[FirebaseRunResultManager] 런 결과 조회 성공: {results.Count}건");
			return (true, null, results);
		}
		catch (Exception e) {
			Debug.LogError($"[FirebaseRunResultManager] 런 결과 조회 실패: {e.Message}");
			return (false, e.Message, null);
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
