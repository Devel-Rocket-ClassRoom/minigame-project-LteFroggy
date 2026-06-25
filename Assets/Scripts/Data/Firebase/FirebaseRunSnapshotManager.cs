using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class FirebaseRunSnapshotManager : MonoBehaviour {
	private const string LogPrefix = "[FirebaseRunSnapshotManager]";

	private FirebaseAuthManager _authManager;
	private FirebaseDatabase _database;

	public void Initialize(FirebaseAuthManager authManager, FirebaseDatabase database) {
		_authManager = authManager;
		_database = database;
	}

	private void OnEnable() {
		GameEvents.OnNodeCompleted += SaveCurrentRunSnapshotSafe;
	}

	private void OnDisable() {
		GameEvents.OnNodeCompleted -= SaveCurrentRunSnapshotSafe;
	}

	private void SaveCurrentRunSnapshotSafe() {
		SaveCurrentRunSnapshot().Forget();
	}

	public async UniTask<(bool success, string error)> SaveCurrentRunSnapshot() {
		if (_database == null) {
			Debug.LogWarning($"{LogPrefix} 현재 런 스냅샷 저장 중단: Firebase Database가 초기화되지 않았습니다.");
			return (false, $"{LogPrefix} Firebase Database가 초기화되지 않았습니다.");
		}

		if (_authManager == null || !_authManager.IsLoggedIn) {
			Debug.LogWarning($"{LogPrefix} 현재 런 스냅샷 저장 중단: 로그인 정보가 없습니다.");
			return (false, $"{LogPrefix} 로그인 정보가 없습니다.");
		}

		try {
			RunSnapshotData snapshot = BuildSnapshotData();
			string json = JsonUtility.ToJson(snapshot);
			await _database.RootReference
				.Child("users")
				.Child(_authManager.UserId)
				.Child("currentRun")
				.SetRawJsonValueAsync(json)
				.AsUniTask();

			Debug.Log($"{LogPrefix} 현재 런 스냅샷 저장 완료");
			return (true, null);
		}
		catch (Exception e) {
			Debug.LogWarning($"{LogPrefix} 현재 런 스냅샷 저장 실패: {e.Message}");
			return (false, e.Message);
		}
	}

	private static RunSnapshotData BuildSnapshotData() {
		GamePlayData data = GamePlayData.Instance;
		NodeData node = data.InGameMapData?.NodeNow;
		(int layer, int index) = FindNodeIndex(data.InGameMapData, node);

		return new RunSnapshotData {
			currentHealth = data.CurrentHealth,
			maxHealth = data.MaxHealth,
			gold = data.Gold,
			totalGoldEarned = data.TotalGoldEarned,
			totalGoldSpent = data.TotalGoldSpent,
			deckCount = data.Deck.Count,
			relicCount = data.Relics.Count,
			cardIds = data.Deck
				.Where(card => card?._cardDefinition != null)
				.Select(card => card._cardDefinition.cardId.ToString())
				.ToArray(),
			relicIds = data.Relics
				.Where(relic => relic != null)
				.Select(relic => relic.relicId)
				.ToArray(),
			currentNodeType = node?.Config != null ? node.Config.Type.ToString() : string.Empty,
			currentNodeLayer = layer,
			currentNodeIndex = index,
			visitedNodeTypes = BuildVisitedNodeTypes(data.InGameMapData),
			savedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
		};
	}

	private static (int layer, int index) FindNodeIndex(InGameMapData mapData, NodeData target) {
		if (mapData?.Nodes == null || target == null)
			return (-1, -1);

		for (int layer = 0; layer < mapData.Nodes.GetLength(0); layer++) {
			for (int index = 0; index < mapData.Nodes.GetLength(1); index++) {
				if (ReferenceEquals(mapData.Nodes[layer, index], target))
					return (layer, index);
			}
		}

		return (-1, -1);
	}

	private static string[] BuildVisitedNodeTypes(InGameMapData mapData) {
		if (mapData?.Nodes == null)
			return Array.Empty<string>();

		var result = new List<string>();
		for (int layer = 0; layer < mapData.Nodes.GetLength(0); layer++) {
			for (int index = 0; index < mapData.Nodes.GetLength(1); index++) {
				NodeData node = mapData.Nodes[layer, index];
				if (node?.Visited == true && node.Config != null)
					result.Add(node.Config.Type.ToString());
			}
		}

		return result.ToArray();
	}
}
