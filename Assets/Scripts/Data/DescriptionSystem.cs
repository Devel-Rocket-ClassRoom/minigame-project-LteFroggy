using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 키워드 강조 및 설명 패널 표시를 담당하는 정적 시스템
// 툴팁 Canvas를 직접 생성하고 DontDestroyOnLoad로 관리하므로 외부 초기화 불필요
public static class DescriptionSystem {
	private static RectTransform _tooltipCanvas;
	private static RectTransform _panelContainer;
	private static readonly List<DescriptionPanelController> _activePanels = new();
	private static readonly Vector2 BottomLeftAnchor = Vector2.zero;
	private static readonly Vector2 TopLeftPivot = new(0f, 1f);
	private static Vector2 _preferredContainerPosition;
	private static Vector2 _fallbackContainerPosition;

	// 최초 접근 시 Canvas 자동 생성 (Lazy Initialization)
	private static RectTransform PanelContainer {
		get {
			if (_panelContainer == null) CreateTooltipCanvas();
			return _panelContainer;
		}
	}

	private static RectTransform TooltipCanvas {
		get {
			if (_tooltipCanvas == null) CreateTooltipCanvas();
			return _tooltipCanvas;
		}
	}

	// 툴팁 전용 Canvas와 패널을 세로로 쌓을 컨테이너를 코드로 생성
	// Sort Order 100으로 항상 최상단에 렌더링됨
	private static void CreateTooltipCanvas() {
		var canvasObj = new GameObject("TooltipCanvas");
		Object.DontDestroyOnLoad(canvasObj);

		var canvas = canvasObj.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 100;
		var scaler = canvasObj.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1020f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 0f;

		_tooltipCanvas = canvasObj.GetComponent<RectTransform>();

		// 패널들이 세로로 쌓이는 컨테이너
		var containerObj = new GameObject("PanelContainer", typeof(RectTransform));
		containerObj.transform.SetParent(canvasObj.transform, false);

		var vlg = containerObj.AddComponent<VerticalLayoutGroup>();
		vlg.childControlWidth = true;
		vlg.childControlHeight = true;
		vlg.spacing = 4f;

		var csf = containerObj.AddComponent<ContentSizeFitter>();
		csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		_panelContainer = containerObj.GetComponent<RectTransform>();
		_panelContainer.anchorMin = BottomLeftAnchor;
		_panelContainer.anchorMax = BottomLeftAnchor;
		_panelContainer.pivot = TopLeftPivot;
		_panelContainer.sizeDelta = new Vector2(300f, 0f);
	}

	// 카드 설명 텍스트의 키워드를 노란색으로 강조하고, 키워드 설명 패널을 source 옆에 표시
	// 강조 처리된 텍스트를 반환하므로, 반환값을 TMP에 넣으면 됨
	public static string ProcessCardText(string text, RectTransform source) {
		SetContainerPosition(source);
		Show(CollectKeywords(text));
		ClampContainerToScreen();
		return ProcessText(text);
	}

	public static void ProcessDescriptionPanel(string title, string description, RectTransform source) {
		Hide();
		SetContainerPosition(source);
		Show(title, description);
		Show(CollectKeywords(description));
		ClampContainerToScreen();
	}

	// 유물의 이름/설명 패널을 표시하고, 설명 내 키워드 패널도 추가로 표시
	public static void ProcessRelicPanel(RelicBase relic, RectTransform source) {
		SetContainerPosition(source);
		Show(relic.displayName, relic.effectDescription);
		Show(CollectKeywords(relic.effectDescription));
		ClampContainerToScreen();
	}

	// 캐릭터에 걸린 상태이상 키워드들의 설명 패널 표시 (중첩 키워드 포함)
	// 캐릭터는 월드 오브젝트이므로 RectTransform이 아닌 스크린 좌표를 받는다
	public static void ProcessStatusPanels(IEnumerable<StatusBase> statuses, Vector2 screenPos) {
		SetContainerPosition(screenPos);
		// 상태이상들은 아이콘까지 써 줘야 함.
		foreach (var status in statuses) {
			ShowWithIcon(status.DescriptionTitle, status.DescriptionContent, status.Icon);	
		}
		ClampContainerToScreen();
	}

	public static void ProcessEnemyIntentAndStatusPanels(
		IReadOnlyList<EnemyAction> actions,
		EnemyActionContext context,
		IEnumerable<StatusBase> statuses,
		Vector2 screenPos
	) {
		Hide();
		SetContainerPosition(screenPos);

		Dictionary<string, string> keywordInfos = new();
		if (actions != null && context != null) {
			foreach (var action in actions) {
				if (action == null) continue;

				string description = action.GetIntentDescriptionWithContext(context);
				ShowWithIcon(action.IntentDescriptionTitle, description, action.IntentIcon);
				AddKeywords(description, keywordInfos);
			}
		}

		if (statuses != null) {
			foreach (var status in statuses) {
				if (status == null) continue;

				ShowWithIcon(status.DescriptionTitle, status.DescriptionContent, status.Icon);
				AddKeywords(status.DescriptionContent, keywordInfos);
			}
		}

		Show(keywordInfos);
		ClampContainerToScreen();
	}

	// 현재 표시 중인 패널을 전부 Pool에 반납
	public static void Hide() {
		foreach (var panel in _activePanels)
			DescriptionPanelPool.Instance.Release(panel);
		_activePanels.Clear();
	}

	// source RectTransform의 화면 기준 bounding box 우상단을 기준으로 패널 컨테이너 위치 설정
	// 카드가 회전된 상태에서도 올바른 위치를 잡기 위해 4개 꼭짓점의 최대 XY를 사용
	private static void SetContainerPosition(RectTransform source) {
		Vector3[] corners = new Vector3[4];
		source.GetWorldCorners(corners);
		float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
		float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
		float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
		float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
		SetContainerPosition(new Vector2(maxX, maxY), new Vector2(minX, minY));
	}

	// 스크린 좌표를 기준으로 패널 컨테이너 위치 설정
	private static void SetContainerPosition(Vector2 screenPos) {
		SetContainerPosition(screenPos, screenPos);
	}

	private static void SetContainerPosition(Vector2 preferredScreenPos, Vector2 fallbackScreenPos) {
		PanelContainer.anchorMin = BottomLeftAnchor;
		PanelContainer.anchorMax = BottomLeftAnchor;
		PanelContainer.pivot = TopLeftPivot;
		_preferredContainerPosition = ScreenToBottomLeftAnchoredPosition(preferredScreenPos);
		_fallbackContainerPosition = ScreenToBottomLeftAnchoredPosition(fallbackScreenPos);
		PanelContainer.anchoredPosition = _preferredContainerPosition;
	}

	private static Vector2 ScreenToBottomLeftAnchoredPosition(Vector2 screenPos) {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			TooltipCanvas,
			screenPos,
			null,
			out Vector2 localPos
		);
		return localPos - TooltipCanvas.rect.min;
	}

	private static void ClampContainerToScreen() {
		if (_activePanels.Count == 0) return;

		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(PanelContainer);
		Canvas.ForceUpdateCanvases();

		Vector2 panelSize = PanelContainer.rect.size;
		Vector2 position = IsOutsideCanvas(_preferredContainerPosition, panelSize)
			? _fallbackContainerPosition
			: _preferredContainerPosition;

		PanelContainer.anchoredPosition = ClampToCanvas(position, panelSize);
	}

	private static bool IsOutsideCanvas(Vector2 topLeftPosition, Vector2 panelSize) {
		Rect canvasRect = TooltipCanvas.rect;
		return topLeftPosition.x < 0f
			|| topLeftPosition.x + panelSize.x > canvasRect.width
			|| topLeftPosition.y > canvasRect.height
			|| topLeftPosition.y - panelSize.y < 0f;
	}

	private static Vector2 ClampToCanvas(Vector2 topLeftPosition, Vector2 panelSize) {
		Rect canvasRect = TooltipCanvas.rect;
		topLeftPosition.x = Mathf.Clamp(
			topLeftPosition.x,
			0f,
			Mathf.Max(0f, canvasRect.width - panelSize.x)
		);

		if (topLeftPosition.y > canvasRect.height) {
			topLeftPosition.y = canvasRect.height;
		}

		if (topLeftPosition.y - panelSize.y < 0f) {
			topLeftPosition.y = Mathf.Min(canvasRect.height, panelSize.y);
		}

		return topLeftPosition;
	}

	private static void Show(Dictionary<string, string> infos) {
		foreach (var (title, desc) in infos)
			Show(title, desc);
	}

	private static void AddKeywords(string text, Dictionary<string, string> keywordInfos) {
		foreach (var (title, desc) in CollectKeywords(text)) {
			if (!keywordInfos.ContainsKey(title)) {
				keywordInfos.Add(title, desc);
			}
		}
	}

	// Pool에서 패널을 꺼내 컨테이너 아래에 붙이고 활성 목록에 추가
	private static void Show(string title, string description) {
		var controller = DescriptionPanelPool.Instance.Get(title, ProcessText(description), PanelContainer);
		_activePanels.Add(controller);
	}
	
	private static void ShowWithIcon(string title, string description, Sprite sprite) {
		var controller = DescriptionPanelPool.Instance.Get(title, ProcessText(description), PanelContainer, sprite);
		_activePanels.Add(controller);
	}

	// 텍스트 내 DescriptionTable 키워드를 노란색(#FFED53) 태그로 래핑
	public static string ProcessText(string text) {
		foreach (var keyword in StringTableManager.DescriptionTable.Keys)
			text = text.Replace(keyword, $"<color=#FFED53>{keyword}</color>");
		return text;
	}

	// BFS로 텍스트 내 키워드를 수집
	// 키워드 설명 내에 또 다른 키워드가 있으면 그것도 재귀적으로 수집 (중복 제거)
	private static Dictionary<string, string> CollectKeywords(string text) {
		var result = new Dictionary<string, string>();
		var queue = new Queue<string>();
		queue.Enqueue(text);
		while (queue.Count > 0) {
			string current = queue.Dequeue();
			foreach (var keyword in StringTableManager.DescriptionTable.Keys) {
				if (current.Contains(keyword) && !result.ContainsKey(keyword)) {
					result.Add(keyword, StringTableManager.DescriptionTable[keyword]);
					// 이 키워드의 설명에도 키워드가 있을 수 있으므로 큐에 추가
					queue.Enqueue(StringTableManager.DescriptionTable[keyword]);
				}
			}
		}
		return result;
	}
}
