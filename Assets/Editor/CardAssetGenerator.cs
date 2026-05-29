using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CardAssetGenerator {
	private const string ActionPath = "Assets/Resources/Datas/Cards/CardAction";
	private const string CardPath = "Assets/Resources/Datas/Cards/CardDescription";

	[MenuItem("Tools/Card/Generate Card Assets")]
	public static void GenerateCardAssets() {
		var actions = CreateCardActionAssets();
		AssetDatabase.SaveAssets();
		CreateCardDefinitionAssets(actions);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		RegisterCardsToRewardPool();
		Debug.Log("카드 에셋 생성 완료");
	}

	[MenuItem("Tools/Card/Register Cards to Reward Pool")]
	public static void RegisterCardsToRewardPool() {
		var gamePlayData = UnityEngine.Object.FindObjectOfType<GamePlayData>();
		if (gamePlayData == null) {
			Debug.LogError("[CardAssetGenerator] GamePlayData를 찾을 수 없습니다. GamePlayData가 포함된 씬을 열고 다시 시도하세요.");
			return;
		}

		// cardId >= 5 인 카드만 보상 풀에 등록 (0~4는 시작 덱)
		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		var rewardCards = new List<CardDefinition>();
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null && card.cardId >= 5) rewardCards.Add(card);
		}

		var so = new SerializedObject(gamePlayData);
		var prop = so.FindProperty("_rewardCardPool");

		// 기존 풀에 없는 카드만 추가 (중복 방지)
		var existingPaths = new HashSet<string>();
		for (int i = 0; i < prop.arraySize; i++) {
			var existing = prop.GetArrayElementAtIndex(i).objectReferenceValue;
			if (existing != null) existingPaths.Add(AssetDatabase.GetAssetPath(existing));
		}

		int added = 0;
		foreach (var card in rewardCards) {
			string path = AssetDatabase.GetAssetPath(card);
			if (existingPaths.Contains(path)) continue;
			prop.InsertArrayElementAtIndex(prop.arraySize);
			prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = card;
			added++;
		}

		so.ApplyModifiedProperties();
		EditorSceneManager.MarkAllScenesDirty();
		Debug.Log($"[CardAssetGenerator] 보상 풀에 카드 {added}장 추가됨 (총 {prop.arraySize}장)");
	}

	private static Dictionary<string, CardAction> CreateCardActionAssets() {
		var map = new Dictionary<string, CardAction>();

		map["Deal6Damage"]           = GetOrCreate<DealDamageCardAction>("Deal6Damage",           a => a.amount = 6);
		map["Deal10Damage"]          = GetOrCreate<DealDamageCardAction>("Deal10Damage",          a => a.amount = 10);
		map["Get18Armor"]            = GetOrCreate<GainArmorCardAction>("Get18Armor",             a => a.amount = 18);
		map["Burn2"]                 = GetOrCreate<BurnCardAction>("Burn2",                       a => a.amount = 2);
		map["Burn4"]                 = GetOrCreate<BurnCardAction>("Burn4",                       a => a.amount = 4);
		map["Draw1Card"]             = GetOrCreate<DrawCardAction>("Draw1Card",                   a => a.amount = 1);
		map["RepeatDamage3x3"]       = GetOrCreate<RepeatDealDamageAction>("RepeatDamage3x3",     a => { a.amount = 3; a.repeat = 3; });
		map["RepeatDamage4x2"]       = GetOrCreate<RepeatDealDamageAction>("RepeatDamage4x2",     a => { a.amount = 4; a.repeat = 2; });
		map["ArmorDamage"]           = GetOrCreate<ArmorDamageCardAction>("ArmorDamage",          a => { });
		map["ResetEnemyArmor"]       = GetOrCreate<ResetEnemyArmorCardAction>("ResetEnemyArmor",  a => { });
		map["BurnCondDamage4Plus6"]  = GetOrCreate<ConditionalBurnBonusDamageCardAction>("BurnCondDamage4Plus6", a => { a.amount = 4; a.bonusAmount = 6; });
		map["LastHitDouble3x4"]      = GetOrCreate<LastHitDoubleRepeatDamageAction>("LastHitDouble3x4",          a => { a.amount = 3; a.repeat = 4; });
		map["RepeatDamageApplyBurn2x4"] = GetOrCreate<RepeatDamageApplyBurnAction>("RepeatDamageApplyBurn2x4",   a => { a.amount = 2; a.repeat = 4; });
		map["AddBurnStacks3"]        = GetOrCreate<AddBurnStacksCardAction>("AddBurnStacks3",     a => a.amount = 3);
		map["HandDefenseCount5"]     = GetOrCreate<HandDefenseCountArmorAction>("HandDefenseCount5", a => a.amount = 5);
		map["BlockAdditionalDraw"]   = GetOrCreate<BlockAdditionalDrawCardAction>("BlockAdditionalDraw", a => { });
		map["ApplyWeakPoint"]        = GetOrCreate<ApplyWeakPointCardAction>("ApplyWeakPoint",    a => { });

		return map;
	}

	private static T GetOrCreate<T>(string name, Action<T> configure) where T : ScriptableObject {
		string path = $"{ActionPath}/{name}.asset";
		var existing = AssetDatabase.LoadAssetAtPath<T>(path);
		if (existing != null) return existing;
		var asset = ScriptableObject.CreateInstance<T>();
		configure(asset);
		AssetDatabase.CreateAsset(asset, path);
		return asset;
	}

	private static void CreateCardDefinitionAssets(Dictionary<string, CardAction> a) {
		var usedIds = new HashSet<int>();
		CollectExistingIds(usedIds);

		// 역습 (5)
		CreateCard("Retaliation", 5, 1, CardTag.Attack, true, new[] { a["ArmorDamage"] }, usedIds);
		// 분쇄 (6)
		CreateCard("Crush", 6, 2, CardTag.Attack, true, new CardAction[] { a["Deal10Damage"], a["ResetEnemyArmor"] }, usedIds);
		// 연소 (7)
		CreateCard("Ignite", 7, 1, CardTag.Attack, true, new[] { a["BurnCondDamage4Plus6"] }, usedIds);
		// 화염검 (8)
		CreateCard("FireSword", 8, 1, CardTag.Fire, true, new CardAction[] { a["Deal6Damage"], a["Burn2"] }, usedIds);
		// 연속 베기 (9)
		CreateCard("ChainSlash", 9, 1, CardTag.MultiHit, true, new[] { a["RepeatDamage3x3"] }, usedIds);
		// 단검 난무 (10)
		CreateCard("DaggerStorm", 10, 1, CardTag.MultiHit, true, new[] { a["RepeatDamage4x2"] }, usedIds);
		// 회오리 (11)
		CreateCard("Whirlwind", 11, 2, CardTag.MultiHit, true, new[] { a["LastHitDouble3x4"] }, usedIds);
		// 불꽃 연타 (12)
		CreateCard("FlamingMultiHit", 12, 2, CardTag.MultiHit, true, new[] { a["RepeatDamageApplyBurn2x4"] }, usedIds);
		// 철벽 (13)
		CreateCard("IronWall", 13, 2, CardTag.Defense, false, new CardAction[] { a["Get18Armor"], a["BlockAdditionalDraw"] }, usedIds);
		// 견고함 (14)
		CreateCard("Fortify", 14, 1, CardTag.Defense, false, new[] { a["HandDefenseCount5"] }, usedIds);
		// 불꽃 손길 (15)
		CreateCard("FlamingTouch", 15, 1, CardTag.Fire, true, new[] { a["Burn4"] }, usedIds);
		// 연소 촉진 (16)
		CreateCard("BurnAccelerate", 16, 1, CardTag.Fire, true, new[] { a["AddBurnStacks3"] }, usedIds);
		// 약점 간파 (17)
		CreateCard("WeakPointFind", 17, 1, CardTag.Util, true, new CardAction[] { a["ApplyWeakPoint"], a["Draw1Card"] }, usedIds);
	}

	private static void CollectExistingIds(HashSet<int> usedIds) {
		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null) usedIds.Add(card.cardId);
		}
	}

	private static void CreateCard(string fileName, int cardId, int cost, CardTag tag, bool needsTarget, CardAction[] actions, HashSet<int> usedIds) {
		string path = $"{CardPath}/{fileName}.asset";
		if (AssetDatabase.LoadAssetAtPath<CardDefinition>(path) != null) {
			Debug.Log($"{fileName}.asset 이미 존재, 건너뜀");
			return;
		}
		if (!usedIds.Add(cardId)) {
			Debug.LogError($"[CardAssetGenerator] cardId {cardId} 중복! {fileName}.asset 생성 취소");
			return;
		}
		var card = ScriptableObject.CreateInstance<CardDefinition>();
		card.cardId = cardId;
		card.rarity = CardRarity.Common;
		card.cost = cost;
		card.tag = tag;
		card.needsTarget = needsTarget;
		card.actions = new List<CardAction>(actions);
		card.icon = LoadIcon(fileName);
		AssetDatabase.CreateAsset(card, path);
		Debug.Log($"{fileName}.asset 생성됨 (cardId: {cardId})");
	}

	private static Sprite LoadIcon(string fileName) {
		string path = $"Assets/Sprites/Cards/{fileName}.png";
		return AssetDatabase.LoadAssetAtPath<Sprite>(path);
	}
}
