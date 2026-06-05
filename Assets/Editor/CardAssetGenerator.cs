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

	[MenuItem("Tools/Card/Migrate Starter Cards to Canonical Actions")]
	public static void MigrateStarterCardsToCanonicalActions() {
		string[] required = { "Deal6Damage", "Draw1Card", "Get5Armor", "Get3Armor", "Draw2Card", "Weakness2" };
		foreach (var name in required) {
			if (AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/{name}.asset") == null) {
				Debug.LogError($"[CardAssetGenerator] 루트 에셋 '{name}.asset' 없음. Generate Card Assets를 먼저 실행하세요.");
				return;
			}
		}

		var deal6Damage = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Deal6Damage.asset");
		var get5Armor = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Get5Armor.asset");
		var get3Armor = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Get3Armor.asset");
		var draw1Card = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Draw1Card.asset");
		var draw2Card = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Draw2Card.asset");
		var weakness2 = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Weakness2.asset");

		UpdateStarterCardActions($"{CardPath}/Attack.asset", new[] { deal6Damage });
		UpdateStarterCardActions($"{CardPath}/Defence.asset", new[] { get5Armor });
		UpdateStarterCardActions($"{CardPath}/Evade.asset", new[] { get3Armor, draw1Card });
		UpdateStarterCardActions($"{CardPath}/Concentrate.asset", new[] { draw2Card });
		UpdateStarterCardActions($"{CardPath}/HuntingSign.asset", new[] { weakness2 });

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("[CardAssetGenerator] 시작 카드 마이그레이션 완료");
	}

	private static void UpdateStarterCardActions(string cardPath, CardAction[] newActions) {
		var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(cardPath);
		if (card == null) {
			Debug.LogWarning($"[CardAssetGenerator] 카드 에셋 없음, 건너뜀: {cardPath}");
			return;
		}

		card.actions = new List<CardAction>(newActions);
		EditorUtility.SetDirty(card);
	}

	[MenuItem("Tools/Card/Register Cards to Reward Pool")]
	public static void RegisterCardsToRewardPool() {
		var gamePlayData = UnityEngine.Object.FindObjectOfType<GamePlayData>();
		if (gamePlayData == null) {
			Debug.LogError("[CardAssetGenerator] GamePlayData를 찾을 수 없습니다. GamePlayData가 포함된 씬을 열고 다시 시도하세요.");
			return;
		}

		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		var rewardCards = new List<CardDefinition>();
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null && card.cardId >= 5) rewardCards.Add(card);
		}

		var so = new SerializedObject(gamePlayData);
		var prop = so.FindProperty("_rewardCardPool");

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
		Debug.Log($"[CardAssetGenerator] 보상 풀 카드 {added}개 추가 (총 {prop.arraySize}개)");
	}

	private static Dictionary<string, CardAction> CreateCardActionAssets() {
		var map = new Dictionary<string, CardAction>();

		map["Get3Armor"] = GetOrCreate<GainArmorCardAction>("Get3Armor", a => a.amount = 3);
		map["Get5Armor"] = GetOrCreate<GainArmorCardAction>("Get5Armor", a => a.amount = 5);
		map["Get6Armor"] = GetOrCreate<GainArmorCardAction>("Get6Armor", a => a.amount = 6);
		map["Get7Armor"] = GetOrCreate<GainArmorCardAction>("Get7Armor", a => a.amount = 7);
		map["Get10Armor"] = GetOrCreate<GainArmorCardAction>("Get10Armor", a => a.amount = 10);
		map["Get18Armor"] = GetOrCreate<GainArmorCardAction>("Get18Armor", a => a.amount = 18);
		map["Get20Armor"] = GetOrCreate<GainArmorCardAction>("Get20Armor", a => a.amount = 20);

		map["Draw1Card"] = GetOrCreate<DrawCardAction>("Draw1Card", a => a.amount = 1);
		map["Draw2Card"] = GetOrCreate<DrawCardAction>("Draw2Card", a => a.amount = 2);

		map["Weakness1"] = GetOrCreate<WeaknessCardAction>("Weakness1", a => a.amount = 1);
		map["Weakness2"] = GetOrCreate<WeaknessCardAction>("Weakness2", a => a.amount = 2);
		map["Vulnerable1"] = GetOrCreate<VulnerableCardAction>("Vulnerable1", a => a.amount = 1);
		map["Strength1"] = GetOrCreate<GetStrengthCardAction>("Strength1", a => a.amount = 1);
		map["Strength2"] = GetOrCreate<GetStrengthCardAction>("Strength2", a => a.amount = 2);

		map["Deal3Damage"] = GetOrCreate<DealDamageCardAction>("Deal3Damage", a => a.amount = 3);
		map["Deal5Damage"] = GetOrCreate<DealDamageCardAction>("Deal5Damage", a => a.amount = 5);
		map["Deal6Damage"] = GetOrCreate<DealDamageCardAction>("Deal6Damage", a => a.amount = 6);
		map["Deal7Damage"] = GetOrCreate<DealDamageCardAction>("Deal7Damage", a => a.amount = 7);
		map["Deal8Damage"] = GetOrCreate<DealDamageCardAction>("Deal8Damage", a => a.amount = 8);
		map["Deal10Damage"] = GetOrCreate<DealDamageCardAction>("Deal10Damage", a => a.amount = 10);
		map["Deal14Damage"] = GetOrCreate<DealDamageCardAction>("Deal14Damage", a => a.amount = 14);
		map["Deal16Damage"] = GetOrCreate<DealDamageCardAction>("Deal16Damage", a => a.amount = 16);
		map["Deal24Damage"] = GetOrCreate<DealDamageCardAction>("Deal24Damage", a => a.amount = 24);

		map["Burn2"] = GetOrCreate<BurnCardAction>("Burn2", a => a.amount = 2);
		map["Burn3"] = GetOrCreate<BurnCardAction>("Burn3", a => a.amount = 3);
		map["Burn4"] = GetOrCreate<BurnCardAction>("Burn4", a => a.amount = 4);
		map["Burn5"] = GetOrCreate<BurnCardAction>("Burn5", a => a.amount = 5);
		map["Burn8"] = GetOrCreate<BurnCardAction>("Burn8", a => a.amount = 8);

		map["RepeatDamage3x3"] = GetOrCreate<RepeatDealDamageAction>("RepeatDamage3x3", a => { a.amount = 3; a.repeat = 3; });
		map["RepeatDamage3x4"] = GetOrCreate<RepeatDealDamageAction>("RepeatDamage3x4", a => { a.amount = 3; a.repeat = 4; });
		map["RepeatDamage4x2"] = GetOrCreate<RepeatDealDamageAction>("RepeatDamage4x2", a => { a.amount = 4; a.repeat = 2; });
		map["ArmorDamage"] = GetOrCreate<ArmorDamageCardAction>("ArmorDamage", a => { });
		map["ResetEnemyArmor"] = GetOrCreate<ResetEnemyArmorCardAction>("ResetEnemyArmor", a => { });
		map["BurnCondDamage4Plus6"] = GetOrCreate<ConditionalBurnBonusDamageCardAction>("BurnCondDamage4Plus6", a => { a.amount = 4; a.bonusAmount = 6; });
		map["LastHitDouble3x4"] = GetOrCreate<LastHitDoubleRepeatDamageAction>("LastHitDouble3x4", a => { a.amount = 3; a.repeat = 4; });
		map["RepeatDamageApplyBurn2x4"] = GetOrCreate<RepeatDamageApplyBurnAction>("RepeatDamageApplyBurn2x4", a => { a.amount = 2; a.repeat = 4; });
		map["AddBurnStacks3"] = GetOrCreate<AddBurnStacksCardAction>("AddBurnStacks3", a => a.amount = 3);
		map["HandDefenseCount5"] = GetOrCreate<HandDefenseCountArmorAction>("HandDefenseCount5", a => a.amount = 5);
		map["BlockAdditionalDraw"] = GetOrCreate<BlockAdditionalDrawCardAction>("BlockAdditionalDraw", a => { });
		map["ApplyHalve"] = GetOrCreate<ApplyHalveCardAction>("ApplyHalve", a => { });

		map["LoseHealth3"] = GetOrCreate<LosePlayerHealthCardAction>("LoseHealth3", a => a.amount = 3);
		map["LoseHealth4"] = GetOrCreate<LosePlayerHealthCardAction>("LoseHealth4", a => a.amount = 4);
		map["LoseHealth5"] = GetOrCreate<LosePlayerHealthCardAction>("LoseHealth5", a => a.amount = 5);
		map["Heal4"] = GetOrCreate<HealPlayerCardAction>("Heal4", a => a.amount = 4);
		map["Execute30Deal6"] = GetOrCreate<ExecuteCardAction>("Execute30Deal6", a => { a.thresholdPercent = 30; a.fallbackDamage = 6; });
		map["Burn2Bonus2"] = GetOrCreate<ConditionalBurnCardAction>("Burn2Bonus2", a => { a.amount = 2; a.bonusAmount = 2; });
		map["Damage12OrBurn4"] = GetOrCreate<ConditionalBurnDamageOrBurnCardAction>("Damage12OrBurn4", a => { a.damageAmount = 12; a.burnAmount = 4; });
		map["Armor10OrDraw1"] = GetOrCreate<ConditionalArmorOrDrawCardAction>("Armor10OrDraw1", a => { a.armorAmount = 10; a.drawAmount = 1; });
		map["LowHealthArmor18Or8"] = GetOrCreate<LowHealthArmorCardAction>("LowHealthArmor18Or8", a => { a.lowHealthArmor = 18; a.normalArmor = 8; a.thresholdPercent = 50; });
		map["NextTurnDraw1"] = GetOrCreate<NextTurnDrawBonusCardAction>("NextTurnDraw1", a => a.amount = 1);
		map["LostHealthDamage6Plus20"] = GetOrCreate<LostHealthBonusDamageCardAction>("LostHealthDamage6Plus20", a => { a.baseDamage = 6; a.lostHealthPercent = 20; });

		return map;
	}

	private static T GetOrCreate<T>(string name, Action<T> configure) where T : ScriptableObject {
		string path = $"{ActionPath}/{name}.asset";
		var asset = AssetDatabase.LoadAssetAtPath<T>(path);
		if (asset == null) {
			asset = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(asset, path);
		}
		configure(asset);
		EditorUtility.SetDirty(asset);
		return asset;
	}

	private static void CreateCardDefinitionAssets(Dictionary<string, CardAction> a) {
		var usedIds = new HashSet<int>();
		CollectExistingIds(usedIds);

		CreateCard("Retaliation", 5, 1, CardTag.Attack, true, CardRarity.Common, CardKeywordType.None, "Retaliation", new[] { a["ArmorDamage"] }, usedIds);
		CreateCard("Crush", 6, 2, CardTag.Attack, true, CardRarity.Common, CardKeywordType.None, "Crush", new[] { a["Deal10Damage"], a["ResetEnemyArmor"] }, usedIds);
		CreateCard("Ignite", 7, 1, CardTag.Attack, true, CardRarity.Common, CardKeywordType.None, "Ignite", new[] { a["BurnCondDamage4Plus6"] }, usedIds);
		CreateCard("FireSword", 8, 1, CardTag.Fire, true, CardRarity.Common, CardKeywordType.None, "FireSword", new[] { a["Deal6Damage"], a["Burn2"] }, usedIds);
		CreateCard("ChainSlash", 9, 1, CardTag.MultiHit, true, CardRarity.Common, CardKeywordType.None, "ChainSlash", new[] { a["RepeatDamage3x3"] }, usedIds);
		CreateCard("DaggerStorm", 10, 1, CardTag.MultiHit, true, CardRarity.Common, CardKeywordType.None, "DaggerStorm", new[] { a["RepeatDamage4x2"] }, usedIds);
		CreateCard("Whirlwind", 11, 2, CardTag.MultiHit, true, CardRarity.Common, CardKeywordType.None, "Whirlwind", new[] { a["LastHitDouble3x4"] }, usedIds);
		CreateCard("FlamingMultiHit", 12, 2, CardTag.MultiHit, true, CardRarity.Common, CardKeywordType.None, "FlamingMultiHit", new[] { a["RepeatDamageApplyBurn2x4"] }, usedIds);
		CreateCard("IronWall", 13, 2, CardTag.Defense, false, CardRarity.Common, CardKeywordType.None, "IronWall", new[] { a["Get18Armor"], a["BlockAdditionalDraw"] }, usedIds);
		CreateCard("Fortify", 14, 1, CardTag.Defense, false, CardRarity.Common, CardKeywordType.None, "Fortify", new[] { a["HandDefenseCount5"] }, usedIds);
		CreateCard("FlamingTouch", 15, 1, CardTag.Fire, true, CardRarity.Common, CardKeywordType.None, "FlamingTouch", new[] { a["Burn4"] }, usedIds);
		CreateCard("BurnAccelerate", 16, 1, CardTag.Fire, true, CardRarity.Common, CardKeywordType.None, "BurnAccelerate", new[] { a["AddBurnStacks3"] }, usedIds);
		CreateCard("HalveFind", 17, 1, CardTag.Util, true, CardRarity.Common, CardKeywordType.None, "HalveFind", new[] { a["ApplyHalve"], a["Draw1Card"] }, usedIds);

		CreateCard("AshBlade", 18, 1, CardTag.Fire, true, CardRarity.Common, CardKeywordType.None, "AshBlade", new[] { a["Deal8Damage"], a["Burn2"] }, usedIds);
		CreateCard("BloodRite", 19, 1, CardTag.Attack, true, CardRarity.Uncommon, CardKeywordType.Exhaust, "BloodRite", new[] { a["LoseHealth3"], a["Deal14Damage"] }, usedIds);
		CreateCard("Execute", 20, 2, CardTag.Attack, true, CardRarity.Rare, CardKeywordType.Exhaust, "Execute", new[] { a["Execute30Deal6"] }, usedIds);
		CreateCard("ChainCleave", 21, 2, CardTag.MultiHit, true, CardRarity.Uncommon, CardKeywordType.None, "ChainCleave", new[] { a["RepeatDamage3x4"] }, usedIds);
		CreateCard("AshPromise", 22, 1, CardTag.Util, false, CardRarity.Uncommon, CardKeywordType.Return, "AshPromise", new[] { a["Draw2Card"] }, usedIds);
		CreateCard("Martyrdom", 23, 0, CardTag.Defense, false, CardRarity.Rare, CardKeywordType.Exhaust, "Martyrdom", new[] { a["LoseHealth5"], a["Get20Armor"] }, usedIds);
		CreateCard("ShieldBash", 24, 1, CardTag.Defense, true, CardRarity.Common, CardKeywordType.None, "ShieldBash", new[] { a["Get6Armor"], a["ArmorDamage"] }, usedIds);
		CreateCard("CatchBreath", 25, 0, CardTag.Util, false, CardRarity.Common, CardKeywordType.None, "CatchBreath", new[] { a["Draw1Card"] }, usedIds);
		CreateCard("EmberSpread", 26, 1, CardTag.Fire, true, CardRarity.Common, CardKeywordType.None, "EmberSpread", new[] { a["Burn2Bonus2"] }, usedIds);
		CreateCard("IronMaceStrike", 27, 2, CardTag.Attack, true, CardRarity.Common, CardKeywordType.None, "IronMaceStrike", new[] { a["Deal16Damage"] }, usedIds);
		CreateCard("RaiseBarricade", 28, 1, CardTag.Defense, false, CardRarity.Common, CardKeywordType.Retain, "RaiseBarricade", new[] { a["Get7Armor"] }, usedIds);
		CreateCard("Overheat", 29, 1, CardTag.Fire, true, CardRarity.Uncommon, CardKeywordType.Overload, "Overheat", new[] { a["Deal10Damage"], a["Burn3"] }, usedIds);
		CreateCard("RefuseRoyalCommand", 30, 0, CardTag.Util, false, CardRarity.Uncommon, CardKeywordType.Innate, "RefuseRoyalCommand", new[] { a["Strength1"], a["Draw1Card"] }, usedIds);
		CreateCard("FirstAid", 31, 1, CardTag.Util, false, CardRarity.Uncommon, CardKeywordType.Exhaust, "FirstAid", new[] { a["Heal4"] }, usedIds);
		CreateCard("Taunt", 32, 1, CardTag.Defense, true, CardRarity.Uncommon, CardKeywordType.None, "Taunt", new[] { a["Get5Armor"], a["Weakness1"] }, usedIds);
		CreateCard("PourOil", 33, 1, CardTag.Fire, true, CardRarity.Uncommon, CardKeywordType.None, "PourOil", new[] { a["Damage12OrBurn4"] }, usedIds);
		CreateCard("SharpenBlade", 34, 1, CardTag.Util, false, CardRarity.Common, CardKeywordType.None, "SharpenBlade", new[] { a["Strength2"] }, usedIds);
		CreateCard("Surge", 35, 1, CardTag.Attack, true, CardRarity.Uncommon, CardKeywordType.Chain, "Surge", new[] { a["Deal7Damage"] }, usedIds);
		CreateCard("LastStand", 36, 1, CardTag.Defense, false, CardRarity.Rare, CardKeywordType.Exhaust, "LastStand", new[] { a["LowHealthArmor18Or8"] }, usedIds);
		CreateCard("Rekindle", 37, 2, CardTag.Fire, true, CardRarity.Rare, CardKeywordType.Return, "Rekindle", new[] { a["Burn5"] }, usedIds);
		CreateCard("Disarm", 38, 1, CardTag.Attack, true, CardRarity.Uncommon, CardKeywordType.None, "Disarm", new[] { a["Deal5Damage"], a["ResetEnemyArmor"] }, usedIds);
		CreateCard("CoverOpening", 39, 1, CardTag.Defense, false, CardRarity.Common, CardKeywordType.None, "CoverOpening", new[] { a["Armor10OrDraw1"] }, usedIds);
		CreateCard("SacrificialFlame", 40, 0, CardTag.Fire, true, CardRarity.Rare, CardKeywordType.Exhaust, "SacrificialFlame", new[] { a["LoseHealth4"], a["Burn8"] }, usedIds);
		CreateCard("QuickStab", 41, 0, CardTag.Attack, true, CardRarity.Common, CardKeywordType.None, "QuickStab", new[] { a["Deal3Damage"] }, usedIds);
		CreateCard("VulnerableStab", 42, 1, CardTag.Attack, true, CardRarity.Uncommon, CardKeywordType.None, "VulnerableStab", new[] { a["Deal5Damage"], a["Vulnerable1"] }, usedIds);
		CreateCard("DefensiveStance", 43, 1, CardTag.Defense, false, CardRarity.Uncommon, CardKeywordType.Innate, "DefensiveStance", new[] { a["Get10Armor"] }, usedIds);
		CreateCard("ContinuousBreath", 44, 1, CardTag.Util, false, CardRarity.Uncommon, CardKeywordType.None, "ContinuousBreath", new[] { a["Draw1Card"], a["NextTurnDraw1"] }, usedIds);
		CreateCard("CrownBreaker", 45, 3, CardTag.Attack, true, CardRarity.Rare, CardKeywordType.Exhaust, "CrownBreaker", new[] { a["Deal24Damage"] }, usedIds);
		CreateCard("FlameBarrier", 46, 2, CardTag.Fire, true, CardRarity.Uncommon, CardKeywordType.None, "FlameBarrier", new[] { a["Get10Armor"], a["Burn3"] }, usedIds);
		CreateCard("BloodRevenge", 47, 1, CardTag.Attack, true, CardRarity.Rare, CardKeywordType.None, "BloodRevenge", new[] { a["LostHealthDamage6Plus20"] }, usedIds);
	}

	private static void CollectExistingIds(HashSet<int> usedIds) {
		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null) usedIds.Add(card.cardId);
		}
	}

	private static void CreateCard(
		string fileName,
		int cardId,
		int cost,
		CardTag tag,
		bool needsTarget,
		CardRarity rarity,
		CardKeywordType keywords,
		string iconName,
		CardAction[] actions,
		HashSet<int> usedIds
	) {
		string path = $"{CardPath}/{fileName}.asset";
		var existingCard = AssetDatabase.LoadAssetAtPath<CardDefinition>(path);
		if (existingCard != null) {
			if (existingCard.cardId != cardId && usedIds.Contains(cardId)) {
				Debug.LogError($"[CardAssetGenerator] cardId {cardId} duplicate! {fileName}.asset update canceled");
				return;
			}

			existingCard.cardId = cardId;
			existingCard.rarity = rarity;
			existingCard.cost = cost;
			existingCard.tag = tag;
			existingCard.needsTarget = needsTarget;
			existingCard.keywords = keywords;
			existingCard.actions = new List<CardAction>(actions);
			existingCard.icon = LoadIcon(iconName);
			EditorUtility.SetDirty(existingCard);
			Debug.Log($"{fileName}.asset updated (cardId: {cardId})");
			return;
		}
		if (!usedIds.Add(cardId)) {
			Debug.LogError($"[CardAssetGenerator] cardId {cardId} 중복! {fileName}.asset 생성 취소");
			return;
		}

		var card = ScriptableObject.CreateInstance<CardDefinition>();
		card.cardId = cardId;
		card.rarity = rarity;
		card.cost = cost;
		card.tag = tag;
		card.needsTarget = needsTarget;
		card.keywords = keywords;
		card.actions = new List<CardAction>(actions);
		card.icon = LoadIcon(iconName);
		AssetDatabase.CreateAsset(card, path);
		Debug.Log($"{fileName}.asset 생성됨 (cardId: {cardId})");
	}

	private static Sprite LoadIcon(string iconName) {
		return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Cards/{iconName}.png");
	}
}
