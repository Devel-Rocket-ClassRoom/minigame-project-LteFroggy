using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : BattleSystemManager {
	[Header("=== 턴 관련 내용 출력할 Text ===")]
	[SerializeField] private TextMeshProUGUI _turnText;

	[Header("=== 턴 종료 버튼 ===")]
	[SerializeField] private Button _turnEndButton;
	[SerializeField] private TextMeshProUGUI _turnEndText;

	[Header("=== Battle Manager ===")]
	[SerializeField] private BattleManager _battleManager;
	
	private readonly float _textPadeDuration = 0.5f;
	private readonly float _textShowDuration = 0.5f;
	private Coroutine _displayTextCoroutine;
	
	// 보일 때는 255, 90, 90, 255
	private readonly Color _textShowColor = new Color(255 / 255, 90 / 255, 90 / 255, 255 / 255);
	// 숨겼을 때는 255, 90, 90, 0
	private readonly Color _textHideColor = new Color(255 / 255, 90 / 255, 90 / 255, 0 / 255);

	private void OnEnable() {
		_turnEndButton.onClick.AddListener(_battleManager.EndPlayerTurn);
		_turnEndText.text = StringTableManager.StringTable["EndTurn"];
	}

	private void OnDisable() {
		_turnEndButton.onClick.RemoveListener(_battleManager.EndPlayerTurn);
	}

	// 턴 관리
	private int _turnCount = 0;
	private bool _isEnemyTurn;

	private void Start() {
		// 시작 시에는 숨기고 시작
		_turnText.color = _textHideColor;
	}

	// 내 턴 시작하면, 텍스트 표시하고 턴 종료 버튼도 표시
	public override void StartPlayerTurn() {
		_turnCount++;
		
		if (_displayTextCoroutine != null) StopCoroutine(_displayTextCoroutine);
		_displayTextCoroutine = StartCoroutine(CoDisplayText($"{_turnCount}번째 턴"));
		
		_turnEndButton.gameObject.SetActive(true);
		_turnEndButton.interactable = true;
	}
	
	// 적 턴 표시
	public override void EndPlayerTurn() {
		if (_displayTextCoroutine != null) StopCoroutine(_displayTextCoroutine);
		_displayTextCoroutine = StartCoroutine(CoDisplayText($"적 턴"));
		
		_turnEndButton.gameObject.SetActive(false);
		_turnEndButton.interactable = false;
	}
	
	private IEnumerator CoDisplayText(string text) {
		_turnText.text = text;
		
		// Pade in
		float timer = 0f;
		while (timer < _textPadeDuration) {
			timer += Time.deltaTime;
			_turnText.color = Color.Lerp(_textHideColor, _textShowColor, timer / _textPadeDuration);
			yield return null;
		}
		
		// Showing
		timer = 0f;
		yield return new WaitForSeconds(_textShowDuration);
		
		// Pade out
		timer = 0f;
		while (timer < _textPadeDuration) {
			timer += Time.deltaTime;
			_turnText.color = Color.Lerp(_textShowColor, _textHideColor, timer / _textPadeDuration);
			yield return null;
		}
		_displayTextCoroutine = null;
	}
}