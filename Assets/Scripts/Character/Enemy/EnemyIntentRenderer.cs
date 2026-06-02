using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyIntentRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private EnemyAction _enemyAction;
	private EnemyActionContext _context;
	private RectTransform _rectTransform;
	private bool _isHovered;

	[Header("=== 의도 아이콘 및 텍스트 ===")]
	[SerializeField] private Image _intentIcon;
	[SerializeField] private TextMeshProUGUI _intentText;

	private void Awake() {
		_rectTransform = GetComponent<RectTransform>();
	}
	
	public void Init(EnemyAction action, EnemyActionContext context) {
		_enemyAction = action;
		
		UpdateIntentInfo(context);
	}
	
	public void UpdateIntentInfo(EnemyActionContext context) {
		_context = context;
		_intentIcon.sprite = _enemyAction.IntentIcon;
		_intentText.text = _enemyAction.GetIntentTextWithContext(context);

		if (_isHovered) {
			ShowIntentPanel();
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		_isHovered = true;
		ShowIntentPanel();
	}

	public void OnPointerExit(PointerEventData eventData) {
		_isHovered = false;
		DescriptionSystem.Hide();
	}

	private void OnDisable() {
		if (!_isHovered) return;

		_isHovered = false;
		DescriptionSystem.Hide();
	}

	private void ShowIntentPanel() {
		DescriptionSystem.ProcessEnemyIntentPanel(_enemyAction, _context, _rectTransform);
	}
}
