using NUnit.Framework;
using UnityEngine;

public class GameContentCatalogTests {
	[Test]
	public void TryGetCardDefinitionFindsCardsByNumericAndStringIds() {
		Assert.That(GameContentCatalog.TryGetCardDefinition(0, out CardDefinition cardByInt), Is.True);
		Assert.That(cardByInt, Is.Not.Null);

		Assert.That(GameContentCatalog.TryGetCardDefinition("0", out CardDefinition cardByString), Is.True);
		Assert.That(cardByString, Is.SameAs(cardByInt));
	}

	[Test]
	public void TryGetRelicFindsRelicsByIdAndTypeName() {
		Assert.That(GameContentCatalog.TryGetRelic("0", out RelicBase relicById), Is.True);
		Assert.That(relicById, Is.TypeOf<Greatsword>());

		Assert.That(GameContentCatalog.TryGetRelic("Greatsword", out RelicBase relicByType), Is.True);
		Assert.That(relicByType, Is.SameAs(relicById));
	}

	[Test]
	public void TryGetEnemyDataFindsEveryEnemyResourceById() {
		EnemyData[] enemies = Resources.LoadAll<EnemyData>("Datas/Enemies/EnemyData");

		Assert.That(enemies, Is.Not.Empty);
		foreach (EnemyData enemy in enemies) {
			Assert.That(GameContentCatalog.TryGetEnemyData(enemy.id, out EnemyData resolved), Is.True);
			Assert.That(resolved, Is.SameAs(enemy));
		}
	}

	[Test]
	public void MissingContentLookupsReturnFalseAndNull() {
		Assert.That(GameContentCatalog.TryGetCardDefinition(-1, out CardDefinition card), Is.False);
		Assert.That(card, Is.Null);
		Assert.That(GameContentCatalog.TryGetCardDefinition("missing", out card), Is.False);
		Assert.That(card, Is.Null);

		Assert.That(GameContentCatalog.TryGetRelic("MissingRelic", out RelicBase relic), Is.False);
		Assert.That(relic, Is.Null);
		Assert.That(GameContentCatalog.TryGetRelicById("Greatsword", out relic), Is.False);
		Assert.That(relic, Is.Null);
		Assert.That(GameContentCatalog.TryGetRelicByTypeName("0", out relic), Is.False);
		Assert.That(relic, Is.Null);

		Assert.That(GameContentCatalog.TryGetEnemyData(-1, out EnemyData enemy), Is.False);
		Assert.That(enemy, Is.Null);
	}
}
