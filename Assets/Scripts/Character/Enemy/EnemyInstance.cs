using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInstance : CharacterBase {
	[Header("=== 타겟팅 박스 Border들 ===")]
	[SerializeField] private Image[] _targetBoxBorders;
	[Header("=== 의도 렌더링 Prefab ===")]
	[SerializeField] private EnemyIntentRenderer _intentRendererPrefab;
	[Header("=== 자기 자신 렌더링할 이미지 ===")]
	[SerializeField] private SpriteRenderer _enemyRenderer;
	[Header("=== 의도가 렌더링될 부모 오브젝트 ===")]
	[SerializeField] private Transform _intentParent;
	
	private BoxCollider2D _collider;
	
	private EnemyData _enemyData;
	private EnemyManager _enemyManager;
	private int _turnCount;
	
	private readonly List<EnemyIntentRenderer> _intentRenderers = new();
	
	private readonly float _actionDuration = 0.5f;
	private readonly float _deadDuration = 1.0f;
	
	// 이번 턴에 수행할 PatternInThisTurn 가져오기
	public EnemyActionPattern PatternInThisTurn => _enemyData.actions[_turnCount % _enemyData.actions.Count];
	
	public void Init(EnemyData data, EnemyManager enemyManager) {
		_enemyData = data;
		_enemyManager = enemyManager;
		_enemyRenderer.sprite = _enemyData.sprite;
		
		_turnCount = 0;
		
		_collider = GetComponent<BoxCollider2D>();

		// base.Init()이 PlayIdleAnimation()을 호출하기 전에 컨트롤러를 할당
		if (_enemyData.animatorController != null)
			GetComponent<Animator>().runtimeAnimatorController = _enemyData.animatorController;
		
		// 시작 시에는 타겟박스 해제 상태로 시작
		SetTargetHighlight(false);
		
		// 시작 시에 의도 표시 표현
		MakeIntentIcon(_enemyManager.GetEnemyActionContext(this));
		
		// 사망 시에는 내 사망 함수 호출하도록
		OnDeath.AddListener(OnDead);
		
		base.Init();
	}
	
	private void OnDead() {
		_enemyManager.DeleteEnemy(this);
		StartCoroutine(CoOnDead());
	}
	
	private IEnumerator CoOnDead() {
		yield return new WaitForSeconds(_deadDuration);
		
		Destroy(gameObject);
	}
	
	public override void OnTurnEnd() {
		base.OnTurnEnd();
		_turnCount++;
		
		// 턴 종료되면, 다음 턴에 내가 할 행동을 머리 위에 띄운다
		MakeIntentIcon(_enemyManager.GetEnemyActionContext(this));
	}
	
	public void MakeIntentIcon(EnemyActionContext context) {
		// 허수아비 방어용 코드
		if (_enemyData == null) return;
		
		// 이전 액션 아이콘 싹 비운다.
		foreach (var intent in _intentRenderers) { Destroy(intent.gameObject); }
		_intentRenderers.Clear();
		
		// 이번 턴 의도 아이콘 리스트로 다시 만들기
		foreach (var action in PatternInThisTurn.actions) {
			var newRenderer = Instantiate(_intentRendererPrefab, _intentParent);
			newRenderer.Init(action, context);
			
			_intentRenderers.Add(newRenderer);
		}
	}
	
	protected override void OnStatusChanged() {
		if (_enemyManager == null) return;
		UpdateIntentIcon(_enemyManager.GetEnemyActionContext(this));
	}

	public void UpdateIntentIcon(EnemyActionContext context) {
		// 허수아비 방어용 코드
		if (_enemyData == null) return;
		
		// 지금 액션 아이콘 싹 다 다시 갱신시킨다.
		foreach (var renderer in _intentRenderers) {
			renderer.UpdateIntentInfo(context);
		}
	}
	
	public IEnumerator CoExecutePattern() {
		foreach (EnemyAction action in PatternInThisTurn.actions) {
			action.Execute(_enemyManager.GetEnemyActionContext(this));
			// 패턴 하나 수행하고 대기시간 (애니메이션 재생 등을 위해)
			yield return new WaitForSeconds(_actionDuration);
		}
	}

	public override void PlayIdleAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Idle");
	}

	public override void PlayAttackAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Attack");
	}

	public override void PlayHitAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Hit");
	}

	public override void PlaySkillAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Skill");
	}

	public override void PlayDeathAnimation() {
		if (_animator == null || _animator.runtimeAnimatorController == null) return;
		_animator.SetTrigger("Death");
	}

	public override void SetHealth() {
		// Data에서 가져와서 체력 세팅
		MaxHealth = _enemyData.health;
		CurrentHealth = MaxHealth;
	}

	public void SetTargetHighlight(bool activate) {
		foreach (var border in _targetBoxBorders) {
			border.enabled = activate;
		}
	}
}