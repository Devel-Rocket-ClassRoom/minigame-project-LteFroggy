using UnityEngine;

/// <summary>
/// 전투 상황 관리에 관여하는 클래스들이 공통으로 가져야 할 함수
/// </summary>
public abstract class BattleSystemManager : MonoBehaviour {
	 public virtual void StartBattle() {}
	 public virtual void StartPlayerTurn() {}
	 public virtual void EndPlayerTurn() {}
}