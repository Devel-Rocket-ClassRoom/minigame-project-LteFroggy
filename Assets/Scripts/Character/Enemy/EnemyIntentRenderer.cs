using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIntentRenderer : MonoBehaviour {
	private EnemyAction _enemyAction;

	[Header("=== 의도 아이콘 및 텍스트 ===")]
	[SerializeField] private Image _intentIcon;
	[SerializeField] private TextMeshProUGUI _intentText;
	
	public void Init(EnemyAction action, EnemyActionContext context) {
		_enemyAction = action;
		
		UpdateIntentInfo(context);
	}
	
	public void UpdateIntentInfo(EnemyActionContext context) {
		_intentIcon.sprite = _enemyAction.IntentIcon;
		_intentText.text = _enemyAction.GetIntentTextWithContext(context);
	}
}