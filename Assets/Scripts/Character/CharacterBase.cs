using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(Animator))]
public abstract class CharacterBase : MonoBehaviour, IHasHealth, IHasBlock {
	public int MaxHealth { get; set; } = 100;
	public int CurrentHealth { get; protected set; }
	public int Block { get; protected set; }
	public BattleManager BattleManager { get; private set; }
	
	public UnityEvent OnDeath;
	public bool IsDead => CurrentHealth <= 0;

	[Header("=== 체력, 방어도 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _healthText;
	[SerializeField] private TextMeshProUGUI _blockText;

	[Header("=== 체력바 이미지 ===")]
	[SerializeField] private Image _healthBarImage;

	[Header("=== 방어도 표시 관련 이미지 ===")] 
	[SerializeField] private Image _blockExistImage;
	[SerializeField] private Image _blockImage;

	[Header("=== 상태 효과 표시용 ===")] 
	[SerializeField] private Transform _statusParent;
	[SerializeField] private StatusRenderer _statusPrefab; 

	[Header("=== 피격 피드백 ===")]
	[SerializeField] private HitFeedbackView _hitFeedbackView;
	
	// 현재 캐릭터에게 걸린 상태 효과 저장용
	private readonly Dictionary<Type, StatusRenderer> _statusRenderers = new();
	protected Animator _animator;
	
	public virtual void Init() {
		// MaxHealth, CurrentHealth는 시작하면서 설정
		SetHealth();
		
		_animator = GetComponent<Animator>();

		if (_hitFeedbackView == null && !TryGetComponent(out _hitFeedbackView)) {
			_hitFeedbackView = gameObject.AddComponent<HitFeedbackView>();
		}
		
		// 시작 시에 Idle로 시작
		PlayIdleAnimation();
	}
	
	public abstract void PlayIdleAnimation();
	public abstract void PlayAttackAnimation();
	public abstract void PlayHitAnimation();
	public abstract void PlaySkillAnimation();
	public abstract void PlayDeathAnimation();
	
	public abstract void SetHealth();

	public void SetBattleManager(BattleManager battleManager) {
		BattleManager = battleManager;
	}

	public void SetCurrentHealth(int current) {
		CurrentHealth = Mathf.Clamp(current, 0, MaxHealth);
		OnHealthChanged();
	}

	public void GetDamage(int amount) {
		GetDamage(amount, null);
	}

	public void GetDamage(int amount, DamageContext damageContext) {
		amount = Mathf.Max(0, amount);
		if (damageContext?.battleManager != null) {
			amount = damageContext.battleManager.RelicManager.ModifyIncomingDamage(damageContext, amount);
		}

		int healthBefore = CurrentHealth;
		bool wasAlive = !IsDead;
		if (damageContext == null || !damageContext.ignoresBlock) {
			int reduceBlockAmount = Math.Min(amount, Block);
			// 피해를 받았을 땐 방어도부터 제거
			amount -= reduceBlockAmount;
			LoseBlock(reduceBlockAmount);
		}

		CurrentHealth -= amount;

		// 체력 0 이하로 내려가지 않게
		CurrentHealth = Mathf.Max(CurrentHealth, 0);
		int actualDamage = Mathf.Max(healthBefore - CurrentHealth, 0);

		_hitFeedbackView?.Play(actualDamage);

		// 맞으면 맞는 애니메이션
		PlayHitAnimation();

		if (actualDamage > 0 && damageContext?.battleManager != null) {
			damageContext.battleManager.RelicManager.OnAfterOwnerDamaged(this, damageContext.source, actualDamage);
		}

		if (wasAlive && IsDead && damageContext?.battleManager != null) {
			if (damageContext.battleManager.RelicManager.TryPreventOwnerDeath(this)) {
				OnHealthChanged();
				return;
			}
			damageContext.battleManager.RelicManager.OnEnemyKilled(damageContext, this);
		}

		if (wasAlive && IsDead) {
			PlayDeathAnimation();
			OnDeath?.Invoke();
		}
		OnHealthChanged();
	}

	//  화상 등의 특별한 효과들은 방어도 고려 않고 바로 체력을 깎는다.
	public void GetDamageWithoutArmor(int amount, DamageContext damageContext = null) {
		var context = damageContext ?? new DamageContext(BattleManager, null, this, null, null, DamageSourceType.Burn, true);
		GetDamage(amount, context);
	}

	public void GetHeal(int amount) {
		CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
		OnHealthChanged();
	}

	protected virtual void OnHealthChanged() { }
	
	public void AddBlock(int amount) { Block += amount; }

	public void LoseBlock(int amount) { Block -= amount; }

	public void ClearBlock() { Block = 0; }
	
	public int CalculateAttackingDamage(int amount) {
		foreach (var status in _statusRenderers) {
			amount = status.Value.Status.ModifyAttackingDamage(amount);
		}
		return amount;
	}
	
	public int CalculateGainingDamage(int amount) {
		foreach (var status in _statusRenderers) {
			amount = status.Value.Status.ModifyGainingDamage(amount);
		}
		return amount;
	}
	
	public int CalculateGainingArmor(int amount) {
		foreach (var status in _statusRenderers) {
			amount = status.Value.Status.ModifyGainingArmor(amount);
		}
		return amount;
	}
	
	public int CalculateGiveBurn(int amount) {
		foreach (var status in _statusRenderers) { amount = status.Value.Status.ModifyGivingBurn(amount); }
		return amount;
	}

	public int CalculateStartingEnergy(int amount) {
		foreach (var status in _statusRenderers) {
			amount = status.Value.Status.ModifyStartingEnergy(amount);
		}

		RefreshStatusesInfo();
		return amount;
	}
	
	public virtual void OnTurnStart() {
		// 매 턴 시작 시 방어도 초기화
		ClearBlock();
		// 턴 시작 시 액션 있는 상태효과 적용
		foreach (var status in _statusRenderers) {
			status.Value.Status.OnTurnStart();
		}
	}
	
	public virtual void OnTurnEnd() {
		// 턴 종료 시 액션 있는 상태효과 적용
		foreach (var status in _statusRenderers) {
			status.Value.Status.OnTurnEnd();
		}
		
		// 상태효과 정보 갱신
		RefreshStatusesInfo();
	}
	
	// 걸린 상태이상이 모두 카드 사용을 허용하면 true (공명 등 통제 상태이상 처리)
	public bool CanUseCard() {
		foreach (var status in _statusRenderers) {
			if (!status.Value.Status.CanUseCard()) return false;
		}
		return true;
	}

	// 카드 사용 시 모든 상태이상에 알림 (공명 등 사용 횟수 추적)
	public void NotifyCardUsed() {
		foreach (var status in _statusRenderers) {
			status.Value.Status.OnCardUsed();
		}
	}

	public bool HasStatus<T>() where T : StatusBase => _statusRenderers.ContainsKey(typeof(T));

	// 현재 걸린 상태이상이 하나라도 있는지
	public bool HasAnyStatus => _statusRenderers.Count > 0;

	// 현재 걸린 상태이상들의 리스트 반환
	public List<StatusBase> GetStatusKeywords() {
		List<StatusBase> statuses = new();
		foreach (var status in _statusRenderers) { statuses.Add(status.Value.Status); }
		
		return statuses;
	}

	// 특정 효과 추가하기
	public void AddStatus(StatusBase status) {
		// 이미 있는 상태이상이면, 합친다.
		if (_statusRenderers.ContainsKey(status.GetType())) { _statusRenderers[status.GetType()].Status.Merge(status); }
		// 없다면, 생성한다
		else {
			var newStatus = Instantiate(_statusPrefab, _statusParent);
			newStatus.Init(status);
			_statusRenderers.Add(status.GetType(), newStatus);
		}
		
		// 상태이상이 더해졌다면, 정보 한번 갱신
		RefreshStatusesInfo();
		OnStatusChanged();
	}

	protected virtual void OnStatusChanged() { }
	
	/// <summary>
	/// 상태이상 정보를 한번 싹 갱신한다.
	/// </summary>
	private void RefreshStatusesInfo() {
		List<Type> pendingDelete = new();
		
		foreach (var pair in _statusRenderers) {
			// 효과가 끝난 값들은 삭제 대기열에 더해두기
			if (!pair.Value.Status.IsActive) { pendingDelete.Add(pair.Key); }
		}
		
		// 삭제할 상태이상은 삭제
		foreach (var status in pendingDelete) {
			// 이미 삭제되었거나 해서 없다면, Destroy 중복으로 호출하지 않도록 continue;
			if (!_statusRenderers.Remove(status, out var statusInstance)) continue;
			Destroy(statusInstance.gameObject);
		}
		
		// 이후 상태 갱신
		foreach (var pair in _statusRenderers) {
			pair.Value.UpdateStatus();
		}
	}

	protected virtual void Update() {
		// 체력 값 갱신
		_healthText.text = $"{CurrentHealth}/{MaxHealth}";
		// 체력바 갱신
		_healthBarImage.fillAmount = (float)CurrentHealth / MaxHealth;
		
		// 방어도가 있다면, 방어도 표시 표현
		if (Block > 0) {
			_blockImage.gameObject.SetActive(true);
			_blockExistImage.gameObject.SetActive(true);
			_blockText.text = $"{Block}";
		} else {
			_blockImage.gameObject.SetActive(false);
			_blockExistImage.gameObject.SetActive(false);
		}
	}
}
