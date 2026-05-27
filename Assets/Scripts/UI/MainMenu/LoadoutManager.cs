using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 로드아웃 패널 관리자
// 역할: 런 시작 전 유물을 선택하는 화면을 담당한다
//   - GamePlayData.AllLoadoutRelics 목록을 기반으로 카드 UI를 동적 생성
//   - 선택한 유물들의 코스트 합계가 _costLimit을 초과하면 선택을 막는다
//   - [출발] 버튼을 누르면 GamePlayData를 초기화하고 선택한 유물을 등록한 뒤 StartScene으로 전환
public class LoadoutManager : MonoBehaviour {
	[SerializeField] private Transform _relicListParent;   // 카드들이 생성될 부모 Transform
	[SerializeField] private GameObject _relicEntryPrefab; // 유물 카드 프리펩 (RelicCard)
	[SerializeField] private TextMeshProUGUI _totalCostText;
	[SerializeField] private Button _startButton;
	[SerializeField] private int _costLimit = 6;

	// 현재 선택된 유물 목록 (HashSet으로 중복 방지)
	private readonly HashSet<RelicBase> _selectedRelics = new();
	private int _totalCost;

	private void OnEnable() {
		// 패널이 열릴 때마다 목록을 새로 그린다
		BuildRelicList();
		_startButton.onClick.AddListener(StartRun);
	}

	private void OnDisable() {
		_startButton.onClick.RemoveListener(StartRun);
	}

	// AllLoadoutRelics를 순회하며 유물 카드 UI를 생성한다
	private void BuildRelicList() {
		// 기존에 생성된 카드 제거
		foreach (Transform child in _relicListParent)
			Destroy(child.gameObject);

		_selectedRelics.Clear();
		_totalCost = 0;
		UpdateCostDisplay();

		foreach (RelicBase relic in GamePlayData.AllLoadoutRelics) {
			// 람다 클로저 캡처 문제 방지용 로컬 변수
			RelicBase captured = relic;
			GameObject entry = Instantiate(_relicEntryPrefab, _relicListParent);

			// 카드 자식 오브젝트에 텍스트 채우기
			SetChildText(entry, "RelicName", captured.displayName);
			SetChildText(entry, "EffectText", captured.effectDescription);
			SetChildText(entry, "CostBadge/CostValue", $"◆ {captured.cost}");
			SetChildText(entry, "RarityBadge/RarityText", StringTableManager.StringTable[captured.rarity.ToString()]);
			SetRelicIcon(entry, captured.icon);

			var toggle = entry.GetComponent<Toggle>();
			toggle.SetIsOnWithoutNotify(false);
			toggle.onValueChanged.AddListener(isOn => OnToggleChanged(captured, toggle, isOn));
		}
	}

	// 카드 프리펩 내부의 TMP 컴포넌트를 경로로 찾아 텍스트를 설정한다
	// path 예시: "RelicName", "CostBadge/CostValue"
	private static void SetChildText(GameObject root, string path, string text) {
		Transform t = root.transform.Find(path);
		if (t != null)
			t.GetComponent<TextMeshProUGUI>().text = text;
	}

	// IconBg의 Image에 유물 아이콘 스프라이트를 설정한다
	// 스프라이트가 있으면 "?" 플레이스홀더 텍스트를 숨긴다
	private static void SetRelicIcon(GameObject root, Sprite icon) {
		Transform iconBg = root.transform.Find("IconBg");
		if (iconBg == null) return;

		if (icon != null) {
			iconBg.GetComponent<Image>().sprite = icon;
			Transform placeholder = iconBg.Find("IconPlaceholder");
			if (placeholder != null) placeholder.gameObject.SetActive(false);
		}
	}

	// 유물 카드 토글 상태가 바뀔 때 호출된다
	private void OnToggleChanged(RelicBase relic, Toggle toggle, bool isOn) {
		if (isOn) {
			// 코스트 초과 시 선택 취소
			if (_totalCost + relic.cost > _costLimit) {
				toggle.SetIsOnWithoutNotify(false);
				return;
			}
			_selectedRelics.Add(relic);
			_totalCost += relic.cost;
		} else {
			if (_selectedRelics.Remove(relic))
				_totalCost -= relic.cost;
		}
		SetHighlight(toggle.gameObject, isOn && _selectedRelics.Contains(relic));
		UpdateCostDisplay();
	}

	// 카드의 SelectHighlight 오브젝트를 켜고 끈다
	private static void SetHighlight(GameObject card, bool active) {
		Transform t = card.transform.Find("SelectHighlight");
		if (t != null) t.gameObject.SetActive(active);
	}

	private void UpdateCostDisplay() {
		_totalCostText.text = $"{_totalCost} / {_costLimit}";
	}

	// 런 시작 처리
	// 1. GamePlayData 초기화 (덱/체력/골드 리셋)
	// 2. 선택한 유물 등록
	// 3. 맵 생성 설정에서 Start 노드의 씬으로 전환
	private void StartRun() {
		GamePlayData.Instance.Reset();
		foreach (RelicBase relic in _selectedRelics)
			GamePlayData.Instance.AddRelic(relic);

		UISceneBootstrapper.Instance.TransitionTo(
			GamePlayData.Instance.MapGeneratingConfig
				.GetConfig(MapNodeType.Start).SceneName);
	}
}
