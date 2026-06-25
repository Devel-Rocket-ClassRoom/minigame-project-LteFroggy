using UnityEngine.Events;

public static class GameEvents {
	public static event UnityAction OnNodeCompleted;
	public static void NodeCompleted() => OnNodeCompleted?.Invoke();

	public static event UnityAction OnRunCleared;
	public static void RunCleared() => OnRunCleared?.Invoke();

	public static event UnityAction<RelicBase> OnRelicTriggered;
	public static void RelicTriggered(RelicBase relic) => OnRelicTriggered?.Invoke(relic);
	
	public static event UnityAction OnNextNodeSelected;
	public static void NextNodeSelected() => OnNextNodeSelected?.Invoke();
}
