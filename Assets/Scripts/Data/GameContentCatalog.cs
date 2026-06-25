using System.Collections.Generic;
using UnityEngine;

public static class GameContentCatalog {
	public const int DefaultUnlockedRelicCount = 3;

	private const string CardDefinitionResourcePath = "Datas/Cards/CardDescription";
	private const string EnemyDataResourcePath = "Datas/Enemies/EnemyData";

	// 로드아웃 화면에서 선택할 수 있는 유물 전체 목록 (게임 내내 고정)
	// 새 유물을 추가하려면 여기에 등록하고 KorStringData.csv에 이름/설명 키를 추가한다
	private static readonly List<RelicBase> _allLoadoutRelics = new() {
		new Greatsword(),
		new Dagger(),
		new ThickShield(),
		new SageGlasses(),
		new RuneOfReturn(),
		new OathOfIncineration(),
		new CrownShardOfDesire(),
		new OathOfRebel(),
		new RustedThornArmor(),
		new FrozenChains(),
		new PromiseOfRing(),
		new HungrySword(),
		new AshGuide(),
		new ForgottenBook(),
		new CrownKiss(),
		new ExecutionChain(),
		new GraveBreath(),
		new SandsOfTime(),
		new SmallBrazier(),
		new AshCache(),
		new WarDrum(),
		new LeadDice(),
		new OldMap(),
		new BloodstainedBandage(),
		new Whetstone(),
		new HeavyBoots(),
		new DryGunpowder(),
		new ColdHeart(),
		new WatchmanBell(),
		new QuickHand(),
		new RoyalEmblem(),
		new BrokenChalice(),
		new BlackCandlestick(),
	};

	private static Dictionary<int, CardDefinition> _cardsById;
	private static Dictionary<string, RelicBase> _relicsByLookupKey;
	private static Dictionary<int, EnemyData> _enemiesById;

	public static IReadOnlyList<RelicBase> AllLoadoutRelics => _allLoadoutRelics;

	public static string[] GetDefaultUnlockedRelicIds() {
		int count = Mathf.Min(DefaultUnlockedRelicCount, _allLoadoutRelics.Count);
		var relicIds = new string[count];
		for (int i = 0; i < count; i++)
			relicIds[i] = _allLoadoutRelics[i].relicId;
		return relicIds;
	}

	public static bool IsDefaultUnlockedLoadoutRelic(RelicBase relic) {
		if (relic == null)
			return false;

		int count = Mathf.Min(DefaultUnlockedRelicCount, _allLoadoutRelics.Count);
		for (int i = 0; i < count; i++) {
			if (_allLoadoutRelics[i].relicId == relic.relicId)
				return true;
		}

		return false;
	}

	public static bool TryGetCardDefinition(int cardId, out CardDefinition definition) {
		return CardsById.TryGetValue(cardId, out definition);
	}

	public static bool TryGetCardDefinition(string cardId, out CardDefinition definition) {
		definition = null;
		return int.TryParse(cardId, out int parsedId)
			&& TryGetCardDefinition(parsedId, out definition);
	}

	public static CardDefinition GetCardDefinitionOrDefault(int cardId) {
		return TryGetCardDefinition(cardId, out CardDefinition definition)
			? definition
			: null;
	}

	public static CardDefinition GetCardDefinitionOrDefault(string cardId) {
		return TryGetCardDefinition(cardId, out CardDefinition definition)
			? definition
			: null;
	}

	public static bool TryGetRelic(string relicIdOrTypeName, out RelicBase relic) {
		if (string.IsNullOrWhiteSpace(relicIdOrTypeName)) {
			relic = null;
			return false;
		}

		return RelicsByLookupKey.TryGetValue(relicIdOrTypeName, out relic);
	}

	public static bool TryGetRelicById(string relicId, out RelicBase relic) {
		if (TryGetRelic(relicId, out relic) && relic.relicId == relicId)
			return true;

		relic = null;
		return false;
	}

	public static bool TryGetRelicByTypeName(string typeName, out RelicBase relic) {
		if (TryGetRelic(typeName, out relic) && relic.GetType().Name == typeName)
			return true;

		relic = null;
		return false;
	}

	public static RelicBase GetRelicOrDefault(string relicIdOrTypeName) {
		return TryGetRelic(relicIdOrTypeName, out RelicBase relic)
			? relic
			: null;
	}

	public static bool TryGetEnemyData(int enemyId, out EnemyData enemyData) {
		return EnemiesById.TryGetValue(enemyId, out enemyData);
	}

	public static EnemyData GetEnemyDataOrDefault(int enemyId) {
		return TryGetEnemyData(enemyId, out EnemyData enemyData)
			? enemyData
			: null;
	}

	private static Dictionary<int, CardDefinition> CardsById {
		get {
			if (_cardsById == null)
				_cardsById = BuildCardLookup();
			return _cardsById;
		}
	}

	private static Dictionary<string, RelicBase> RelicsByLookupKey {
		get {
			if (_relicsByLookupKey == null)
				_relicsByLookupKey = BuildRelicLookup();
			return _relicsByLookupKey;
		}
	}

	private static Dictionary<int, EnemyData> EnemiesById {
		get {
			if (_enemiesById == null)
				_enemiesById = BuildEnemyLookup();
			return _enemiesById;
		}
	}

	private static Dictionary<int, CardDefinition> BuildCardLookup() {
		var lookup = new Dictionary<int, CardDefinition>();
		foreach (CardDefinition card in Resources.LoadAll<CardDefinition>(CardDefinitionResourcePath)) {
			if (card == null || lookup.ContainsKey(card.cardId)) continue;
			lookup.Add(card.cardId, card);
		}

		return lookup;
	}

	private static Dictionary<string, RelicBase> BuildRelicLookup() {
		var lookup = new Dictionary<string, RelicBase>();
		foreach (RelicBase relic in _allLoadoutRelics) {
			if (relic == null) continue;
			AddRelicLookupKey(lookup, relic.relicId, relic);
			AddRelicLookupKey(lookup, relic.GetType().Name, relic);
		}

		return lookup;
	}

	private static void AddRelicLookupKey(Dictionary<string, RelicBase> lookup, string key, RelicBase relic) {
		if (string.IsNullOrWhiteSpace(key) || lookup.ContainsKey(key)) return;
		lookup.Add(key, relic);
	}

	private static Dictionary<int, EnemyData> BuildEnemyLookup() {
		var lookup = new Dictionary<int, EnemyData>();
		foreach (EnemyData enemyData in Resources.LoadAll<EnemyData>(EnemyDataResourcePath)) {
			if (enemyData == null || lookup.ContainsKey(enemyData.id)) continue;
			lookup.Add(enemyData.id, enemyData);
		}

		return lookup;
	}
}
