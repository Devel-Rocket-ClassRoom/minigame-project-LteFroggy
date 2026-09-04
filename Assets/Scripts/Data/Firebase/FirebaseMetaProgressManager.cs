using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class FirebaseMetaProgressManager : MonoBehaviour {
	private const string LogPrefix = "[FirebaseMetaProgressManager]";

	private FirebaseAuthManager _authManager;
	private FirebaseDatabase _database;

	public MetaProgressData Current { get; private set; }
	public bool HasData => Current != null;

	public event Action<MetaProgressData> OnMetaProgressChanged;

	public void Initialize(FirebaseAuthManager authManager, FirebaseDatabase database) {
		_authManager = authManager;
		_database = database;
	}

	public async UniTask<(bool success, string error)> LoadOrCreateMetaProgress() {
		if (!CanUseDatabase(out string error))
			return (false, error);

		try {
			DataSnapshot snapshot = await MetaProgressReference()
				.GetValueAsync()
				.AsUniTask();

			Current = snapshot.Exists && !string.IsNullOrWhiteSpace(snapshot.GetRawJsonValue())
				? JsonUtility.FromJson<MetaProgressData>(snapshot.GetRawJsonValue())
				: CreateDefaultMetaProgress();

			bool changed = EnsureDefaultUnlocks(Current);
			if (!snapshot.Exists || changed)
				await SaveMetaProgress();
			else
				NotifyChanged();

			Debug.Log($"{LogPrefix} 메타 진행 로드 완료");
			return (true, null);
		}
		catch (Exception e) {
			Debug.LogWarning($"{LogPrefix} 메타 진행 로드 실패: {e.Message}");
			return (false, e.Message);
		}
	}

	public async UniTask<(bool success, string error)> AddGold(int amount) {
		if (amount <= 0)
			return (true, null);

		var (loaded, loadError) = await EnsureLoaded();
		if (!loaded)
			return (false, loadError);

		Current.gold += amount;
		return await SaveMetaProgress();
	}

	public async UniTask<(bool success, string error)> TryPurchaseRelic(RelicBase relic) {
		if (relic == null)
			return (false, "구매할 유물을 찾을 수 없습니다.");

		var (loaded, loadError) = await EnsureLoaded();
		if (!loaded)
			return (false, loadError);

		if (IsRelicUnlocked(relic))
			return (false, "이미 보유한 유물입니다.");

		int price = GetRelicPrice(relic);
		if (Current.gold < price)
			return (false, "골드가 부족합니다.");

		Current.gold -= price;
		AddUnique(ref Current.unlockedRelicIds, relic.relicId);
		AddUnique(ref Current.purchasedRelicIds, relic.relicId);
		return await SaveMetaProgress();
	}

	public bool IsRelicUnlocked(RelicBase relic) {
		if (relic == null)
			return false;

		if (Current == null)
			return GameContentCatalog.IsDefaultUnlockedLoadoutRelic(relic);

		return Contains(Current.unlockedRelicIds, relic.relicId);
	}

	public static int GetRelicPrice(RelicBase relic) {
		if (relic == null)
			return 0;

		return relic.rarity switch {
			RelicRarity.Common => 80,
			RelicRarity.Rare => 140,
			RelicRarity.Epic => 220,
			RelicRarity.Legendary => 350,
			_ => 100
		};
	}

	private async UniTask<(bool success, string error)> EnsureLoaded() {
		if (Current != null)
			return (true, null);

		return await LoadOrCreateMetaProgress();
	}

	private async UniTask<(bool success, string error)> SaveMetaProgress() {
		if (!CanUseDatabase(out string error))
			return (false, error);

		try {
			Current.updatedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			string json = JsonUtility.ToJson(Current);
			await MetaProgressReference()
				.SetRawJsonValueAsync(json)
				.AsUniTask();
			NotifyChanged();
			return (true, null);
		}
		catch (Exception e) {
			Debug.LogWarning($"{LogPrefix} 메타 진행 저장 실패: {e.Message}");
			return (false, e.Message);
		}
	}

	private bool CanUseDatabase(out string error) {
		if (_database == null) {
			error = $"{LogPrefix} Firebase Database가 초기화되지 않았습니다.";
			return false;
		}

		if (_authManager == null || !_authManager.IsLoggedIn) {
			error = $"{LogPrefix} 로그인 정보가 없습니다.";
			return false;
		}

		error = null;
		return true;
	}

	private DatabaseReference MetaProgressReference() {
		return _database.RootReference
			.Child("users")
			.Child(_authManager.UserId)
			.Child("metaProgress");
	}

	private static MetaProgressData CreateDefaultMetaProgress() {
		return new MetaProgressData {
			gold = 0,
			unlockedRelicIds = GameContentCatalog.GetDefaultUnlockedRelicIds(),
			purchasedRelicIds = Array.Empty<string>(),
			updatedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
		};
	}

	private static bool EnsureDefaultUnlocks(MetaProgressData data) {
		if (data == null)
			return false;

		bool changed = false;
		foreach (string relicId in GameContentCatalog.GetDefaultUnlockedRelicIds()) {
			if (AddUnique(ref data.unlockedRelicIds, relicId))
				changed = true;
		}

		data.purchasedRelicIds ??= Array.Empty<string>();
		return changed;
	}

	private void NotifyChanged() {
		OnMetaProgressChanged?.Invoke(Current);
	}

	private static bool Contains(string[] values, string value) {
		if (values == null || string.IsNullOrWhiteSpace(value))
			return false;

		foreach (string current in values) {
			if (current == value)
				return true;
		}

		return false;
	}

	private static bool AddUnique(ref string[] values, string value) {
		if (string.IsNullOrWhiteSpace(value) || Contains(values, value))
			return false;

		var list = values != null ? new List<string>(values) : new List<string>();
		list.Add(value);
		values = list.ToArray();
		return true;
	}
}
