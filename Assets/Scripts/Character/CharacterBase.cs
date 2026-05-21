using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterBase : MonoBehaviour, IHasHealth, IHasBlock {
	public int MaxHealth { get; set; } = 100;
	public int CurrentHealth { get; protected set; }
	public int Block { get; protected set; }

	[Header("=== 체력, 방어도 텍스트 ===")]
	[SerializeField] private TextMeshProUGUI _healthText;
	[SerializeField] private TextMeshProUGUI _blockText;

	[Header("=== 체력바 이미지 ===")]
	[SerializeField] private Image _healthBarImage;

	[Header("=== 방어도 표시 관련 이미지 ===")] 
	[SerializeField] private Image _blockExistImage;
	[SerializeField] private Image _blockImage;
	
	// 걸린 강화효과, 약화효과 등 저장
	public List<StatusBase> Statuses = new List<StatusBase>();
	
	// MaxHealth, CurrentHealth는 시작하면서 PlayerData에서 받아오기
	public virtual void Awake() {
		SetHealth();
	}
	
	public abstract void SetHealth();

	public void GetDamage(int amount) {
		int reduceBlockAmount = Math.Min(amount, Block);
		// 피해를 받았을 땐 방어도부터 제거 
		amount -= reduceBlockAmount;
		LoseBlock(reduceBlockAmount);
		
		CurrentHealth -= amount;
	}

	public void GetHeal(int amount) { CurrentHealth += amount; }
	
	public void AddBlock(int amount) { Block += amount; }

	public void LoseBlock(int amount) { Block -= amount; }

	public void ClearBlock() { Block = 0; }
	
	public int CalculateAttackingDamage(int amount) {
		foreach (StatusBase status in Statuses) {
			amount = status.ModifyAttackingDamage(amount);
		}
		return amount;
	}
	
	public int CalculateDefendingDamage(int amount) {
		foreach (StatusBase status in Statuses) {
			amount = status.ModifyDefendingDamage(amount);
		}
		return amount;
	}
	
	public int CalculateGainingArmor(int amount) {
		foreach (StatusBase status in Statuses) {
			amount = status.ModifyGainingArmor(amount);
		}
		return amount;
	}
	
	public void OnTurnStart() {
		// 매 턴 시작 시 방어도 초기화
		ClearBlock();
		foreach (StatusBase status in Statuses) {
			status.OnTurnStart();
		}
	}
	
	public void OnTurnEnd() {
		foreach (StatusBase status in Statuses) {
			status.OnTurnEnd();
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