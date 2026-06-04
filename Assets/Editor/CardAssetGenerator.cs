using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CardAssetGenerator {
	// 생성된 CardAction 에셋이 저장되는 위치.
	// CardDefinition은 이 액션 에셋들을 참조해서 카드 효과 목록을 구성한다.
	private const string ActionPath = "Assets/Resources/Datas/Cards/CardAction";
	// 생성된 CardDefinition 에셋이 저장되는 위치.
	// RegisterCardsToRewardPool도 이 폴더를 기준으로 보상 후보 카드를 찾는다.
	private const string CardPath = "Assets/Resources/Datas/Cards/CardDescription";

	[MenuItem("Tools/Card/Generate Card Assets")]
	public static void GenerateCardAssets() {
		// 전체 생성 메뉴의 진입점.
		// 순서가 중요하다: 카드 정의(CardDefinition)가 액션(CardAction)을 참조하므로
		// 액션 에셋을 먼저 만들고, 그 결과를 Dictionary로 받아 카드 정의 생성에 넘긴다.
		var actions = CreateCardActionAssets();
		AssetDatabase.SaveAssets();

		// 위에서 만든 CardAction 에셋들을 조합해 실제 카드 데이터 에셋을 만든다.
		// 각 CardDefinition에는 비용, 태그, 타겟 필요 여부, 실행할 액션 목록이 들어간다.
		CreateCardDefinitionAssets(actions);
		AssetDatabase.SaveAssets();

		// Unity 에셋 DB를 갱신한 뒤, 새로 생긴 CardDefinition까지 포함해서 보상 풀을 갱신한다.
		AssetDatabase.Refresh();
		RegisterCardsToRewardPool();
		Debug.Log("카드 에셋 생성 완료");
	}

	[MenuItem("Tools/Card/Migrate Starter Cards to Canonical Actions")]
	public static void MigrateStarterCardsToCanonicalActions() {
		// 사전 검증: 루트 canonical 에셋 6개가 모두 존재해야 한다.
		// 없으면 Tools/Card/Generate Card Assets를 먼저 실행해야 한다.
		string[] required = { "Deal6Damage", "Draw1Card", "Get5Armor", "Get3Armor", "Draw2Card", "Weakness2" };
		foreach (var name in required) {
			if (AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/{name}.asset") == null) {
				Debug.LogError($"[CardAssetGenerator] 루트 에셋 '{name}.asset' 없음. Generate Card Assets를 먼저 실행하세요.");
				return;
			}
		}

		// canonical 에셋 로드
		var deal6Damage = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Deal6Damage.asset");
		var get5Armor   = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Get5Armor.asset");
		var get3Armor   = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Get3Armor.asset");
		var draw1Card   = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Draw1Card.asset");
		var draw2Card   = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Draw2Card.asset");
		var weakness2   = AssetDatabase.LoadAssetAtPath<CardAction>($"{ActionPath}/Weakness2.asset");

		// 5개 시작 카드의 actions를 canonical 루트 에셋으로 교체한다.
		// Evade는 [Get3Armor, Draw1Card] 순서를 유지한다.
		UpdateStarterCardActions($"{CardPath}/Attack.asset",      new[] { deal6Damage });
		UpdateStarterCardActions($"{CardPath}/Defence.asset",     new[] { get5Armor });
		UpdateStarterCardActions($"{CardPath}/Evade.asset",       new[] { get3Armor, draw1Card });
		UpdateStarterCardActions($"{CardPath}/Concentrate.asset", new[] { draw2Card });
		UpdateStarterCardActions($"{CardPath}/HuntingSign.asset", new[] { weakness2 });

		// 참조 교체를 디스크에 반영한 뒤 레거시 에셋을 삭제한다.
		// 순서가 중요하다: SaveAssets 전에 삭제하면 교체된 참조가 누락될 수 있다.
		AssetDatabase.SaveAssets();

		string[] legacyAssets = {
			$"{ActionPath}/DealDamage/Deal6Damage.asset",
			$"{ActionPath}/DrawCard/Draw1Card.asset",
			$"{ActionPath}/DrawCard/Draw2Card.asset",
			$"{ActionPath}/GetArmor/Get3Armor.asset",
			$"{ActionPath}/GetArmor/Get5Armor.asset",
			$"{ActionPath}/GIveWeakness/Weakness 2.asset",
		};
		foreach (var path in legacyAssets) {
			if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) != null) {
				AssetDatabase.DeleteAsset(path);
				Debug.Log($"[CardAssetGenerator] 레거시 에셋 삭제: {path}");
			}
		}

		// 레거시 에셋 삭제 후 빈 폴더를 정리한다.
		string[] legacyFolders = {
			$"{ActionPath}/DealDamage",
			$"{ActionPath}/DrawCard",
			$"{ActionPath}/GetArmor",
			$"{ActionPath}/GIveWeakness",
		};
		foreach (var folder in legacyFolders) {
			if (AssetDatabase.IsValidFolder(folder)) {
				AssetDatabase.DeleteAsset(folder);
				Debug.Log($"[CardAssetGenerator] 빈 폴더 삭제: {folder}");
			}
		}

		AssetDatabase.Refresh();
		Debug.Log("[CardAssetGenerator] 시작 카드 마이그레이션 완료");
	}

	private static void UpdateStarterCardActions(string cardPath, CardAction[] newActions) {
		var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(cardPath);
		if (card == null) {
			Debug.LogWarning($"[CardAssetGenerator] 카드 에셋 없음, 건너뜀: {cardPath}");
			return;
		}
		// CardDefinition.actions는 public List<CardAction>이므로 직접 할당 후 SetDirty로 변경을 알린다.
		card.actions = new List<CardAction>(newActions);
		EditorUtility.SetDirty(card);
		Debug.Log($"[CardAssetGenerator] {System.IO.Path.GetFileNameWithoutExtension(cardPath)} 액션 교체 완료");
	}

	[MenuItem("Tools/Card/Register Cards to Reward Pool")]
	public static void RegisterCardsToRewardPool() {
		// GamePlayData는 프로젝트 에셋이 아니라 씬에 배치된 MonoBehaviour이므로,
		// 이 메뉴를 실행할 때 GamePlayData가 포함된 씬이 열려 있어야 한다.
		var gamePlayData = UnityEngine.Object.FindObjectOfType<GamePlayData>();
		if (gamePlayData == null) {
			Debug.LogError("[CardAssetGenerator] GamePlayData를 찾을 수 없습니다. GamePlayData가 포함된 씬을 열고 다시 시도하세요.");
			return;
		}

		// cardId >= 5 인 카드만 보상 풀에 등록한다.
		// 0~4번 카드는 시작 덱 카드로 예약되어 있어서, 보상 카드 후보에 섞지 않는다.
		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		var rewardCards = new List<CardDefinition>();
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null && card.cardId >= 5) rewardCards.Add(card);
		}

		// _rewardCardPool은 private SerializeField라 직접 접근할 수 없다.
		// SerializedObject/SerializedProperty를 사용하면 인스펙터 직렬화 필드를 안전하게 수정할 수 있다.
		var so = new SerializedObject(gamePlayData);
		var prop = so.FindProperty("_rewardCardPool");

		// 기존 풀에 없는 카드만 추가한다.
		// UnityEngine.Object 참조 자체보다 에셋 경로를 비교하면 같은 에셋을 더 안정적으로 중복 판별할 수 있다.
		var existingPaths = new HashSet<string>();
		for (int i = 0; i < prop.arraySize; i++) {
			var existing = prop.GetArrayElementAtIndex(i).objectReferenceValue;
			if (existing != null) existingPaths.Add(AssetDatabase.GetAssetPath(existing));
		}

		// 보상 후보로 모은 CardDefinition 중 아직 풀에 없는 것만 배열 끝에 붙인다.
		// InsertArrayElementAtIndex 뒤에는 마지막 요소에 실제 카드 참조를 직접 넣어야 한다.
		int added = 0;
		foreach (var card in rewardCards) {
			string path = AssetDatabase.GetAssetPath(card);
			if (existingPaths.Contains(path)) continue;
			prop.InsertArrayElementAtIndex(prop.arraySize);
			prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = card;
			added++;
		}

		// SerializedObject 변경분을 실제 GamePlayData 컴포넌트에 반영하고,
		// 씬이 수정되었음을 표시해 사용자가 저장할 수 있게 한다.
		so.ApplyModifiedProperties();
		EditorSceneManager.MarkAllScenesDirty();
		Debug.Log($"[CardAssetGenerator] 보상 풀에 카드 {added}장 추가됨 (총 {prop.arraySize}장)");
	}

	private static Dictionary<string, CardAction> CreateCardActionAssets() {
		// 카드 효과 에셋을 이름으로 찾아 쓸 수 있게 모아둔다.
		// 아래 key들은 CreateCardDefinitionAssets에서 카드별 액션 조합을 만들 때 사용한다.
		var map = new Dictionary<string, CardAction>();

		// GetOrCreate는 기존 에셋이 있어도 configure를 재적용하므로, 생성기 코드가 단일 진실 소스가 된다.
		// 시작 카드(cardId 0~4) 액션도 생성기 관리 대상으로 통합해 루트 경로를 canonical로 확립한다.
		map["Get5Armor"]  = GetOrCreate<GainArmorCardAction>("Get5Armor",  a => a.amount = 5);
		map["Get3Armor"]  = GetOrCreate<GainArmorCardAction>("Get3Armor",  a => a.amount = 3);
		map["Draw2Card"]  = GetOrCreate<DrawCardAction>("Draw2Card",       a => a.amount = 2);
		map["Weakness2"]  = GetOrCreate<WeaknessCardAction>("Weakness2",   a => a.amount = 2);

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
		map["ApplyHalve"]            = GetOrCreate<ApplyHalveCardAction>("ApplyHalve",        a => { });

		return map;
	}

	private static T GetOrCreate<T>(string name, Action<T> configure) where T : ScriptableObject {
		string path = $"{ActionPath}/{name}.asset";

		// 기존 에셋이 있어도 configure를 다시 적용한다.
		// 이 덕분에 생성기 코드의 수치가 바뀌었을 때 메뉴를 재실행하면 에셋에 동기화된다.
		// 기존 에셋 참조(GUID)는 유지되므로 카드 정의의 액션 참조가 깨지지 않는다.
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
		// cardId는 저장/문자열 키와 연결되는 식별자라 중복되면 안 된다.
		// 먼저 폴더 안의 기존 CardDefinition들을 스캔해서 이미 사용 중인 id를 수집한다.
		var usedIds = new HashSet<int>();
		CollectExistingIds(usedIds);

		// CreateCard 인자 순서:
		// fileName: 생성될 .asset 파일명 및 카드 아이콘 파일명
		// cardId: 카드 이름/텍스트 키(Card{cardId}Name 등)와 연결되는 고유 id
		// cost: 카드 사용 에너지 비용
		// tag: 카드 분류
		// needsTarget: 사용 시 적 타겟 선택이 필요한지 여부
		// actions: 실제 실행될 CardAction 목록. CardUseManager가 이 순서대로 Execute한다.

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
		CreateCard("HalveFind", 17, 1, CardTag.Util, true, new CardAction[] { a["ApplyHalve"], a["Draw1Card"] }, usedIds);
	}

	private static void CollectExistingIds(HashSet<int> usedIds) {
		// 새로 만들 카드뿐 아니라 이미 폴더에 존재하는 카드까지 포함해 id 충돌을 막는다.
		var guids = AssetDatabase.FindAssets("t:CardDefinition", new[] { CardPath });
		foreach (var guid in guids) {
			var card = AssetDatabase.LoadAssetAtPath<CardDefinition>(AssetDatabase.GUIDToAssetPath(guid));
			if (card != null) usedIds.Add(card.cardId);
		}
	}

	private static void CreateCard(string fileName, int cardId, int cost, CardTag tag, bool needsTarget, CardAction[] actions, HashSet<int> usedIds) {
		string path = $"{CardPath}/{fileName}.asset";

		// 파일이 이미 있으면 같은 카드를 다시 만들지 않는다.
		// 이 스크립트는 반복 실행될 수 있는 에디터 도구라, 생성 작업은 기본적으로 멱등적으로 동작해야 한다.
		if (AssetDatabase.LoadAssetAtPath<CardDefinition>(path) != null) {
			Debug.Log($"{fileName}.asset 이미 존재, 건너뜀");
			return;
		}

		// 파일명은 달라도 cardId가 같으면 문자열 테이블 키와 저장 데이터가 충돌할 수 있으므로 생성을 막는다.
		if (!usedIds.Add(cardId)) {
			Debug.LogError($"[CardAssetGenerator] cardId {cardId} 중복! {fileName}.asset 생성 취소");
			return;
		}

		// CardDefinition은 카드의 정적 데이터다.
		// 런타임에서는 CardInstance가 이 정의를 감싸고, 실제 사용 시 actions가 순서대로 실행된다.
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
		// 카드 정의 파일명과 아이콘 파일명을 맞춰 둔 규칙을 사용한다.
		// 예: FireSword.asset은 Assets/Sprites/Cards/FireSword.png를 아이콘으로 찾는다.
		string path = $"Assets/Sprites/Cards/{fileName}.png";
		return AssetDatabase.LoadAssetAtPath<Sprite>(path);
	}
}
