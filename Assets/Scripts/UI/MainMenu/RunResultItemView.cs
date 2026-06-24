using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class RunResultItemView : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI _resultText;
	[SerializeField] private TextMeshProUGUI _summaryText;
	[SerializeField] private TextMeshProUGUI _cardIdsText;
	[SerializeField] private TextMeshProUGUI _relicIdsText;
	[SerializeField] private TextMeshProUGUI _enemyIdsText;

	public void SetPlaceholder() {
		if (_resultText != null) _resultText.text = "런 결과";
		if (_summaryText != null) _summaryText.text = "저장된 런 결과를 불러오면 여기에 표시됩니다.";
		if (_cardIdsText != null) _cardIdsText.text = "카드: -";
		if (_relicIdsText != null) _relicIdsText.text = "유물: -";
		if (_enemyIdsText != null) _enemyIdsText.text = "적: -";
	}

	public void SetData(RunResultData data) {
		if (data == null) {
			SetPlaceholder();
			return;
		}

		if (_resultText != null) _resultText.text = string.IsNullOrEmpty(data.result) ? "런 결과" : data.result;
		if (_summaryText != null)
			_summaryText.text = $"{FormatSavedAt(data.savedAtUnixTime)}  HP {data.currentHealth}/{data.maxHealth}  Gold {data.gold}";
		if (_cardIdsText != null) _cardIdsText.text = $"카드: {FormatCards(data.cardIds)}";
		if (_relicIdsText != null) _relicIdsText.text = $"유물: {FormatRelics(data.relicIds)}";
		if (_enemyIdsText != null) _enemyIdsText.text = $"적: {FormatEnemies(data.enemyIds)}";
	}

	private static string FormatCards(string[] cardIds) {
		if (cardIds == null || cardIds.Length == 0)
			return "-";

		return string.Join(", ", cardIds.Select(FormatCard));
	}

	private static string FormatCard(string cardId) {
		if (GameContentCatalog.TryGetCardDefinition(cardId, out CardDefinition definition))
			return GetCardName(definition);

		return string.IsNullOrWhiteSpace(cardId) ? "알 수 없는 카드" : $"Card {cardId}";
	}

	private static string GetCardName(CardDefinition definition) {
		try {
			return definition.StringCardName;
		}
		catch {
			return $"Card {definition.cardId}";
		}
	}

	private static string FormatRelics(string[] relicIds) {
		if (relicIds == null || relicIds.Length == 0)
			return "-";

		return string.Join(", ", relicIds.Select(FormatRelic));
	}

	private static string FormatRelic(string relicId) {
		if (GameContentCatalog.TryGetRelic(relicId, out RelicBase relic))
			return GetRelicName(relic);

		return string.IsNullOrWhiteSpace(relicId) ? "알 수 없는 유물" : relicId;
	}

	private static string GetRelicName(RelicBase relic) {
		try {
			return relic.displayName;
		}
		catch {
			return relic.GetType().Name;
		}
	}

	private static string FormatEnemies(int[] enemyIds) {
		if (enemyIds == null || enemyIds.Length == 0)
			return "-";

		return string.Join(", ", enemyIds.Select(FormatEnemy));
	}

	private static string FormatEnemy(int enemyId) {
		if (GameContentCatalog.TryGetEnemyData(enemyId, out EnemyData enemy) && !string.IsNullOrWhiteSpace(enemy.name))
			return enemy.name;

		return $"Enemy {enemyId}";
	}

	private static string FormatSavedAt(long unixTime) {
		if (unixTime <= 0)
			return "저장 시간 없음";

		return DateTimeOffset.FromUnixTimeSeconds(unixTime)
			.ToLocalTime()
			.ToString("yyyy-MM-dd HH:mm");
	}
}
