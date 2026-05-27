using UnityEngine.Events;

public static class GameEvents {
	public static event UnityAction OnNodeCompleted;
	public static void NodeCompleted() => OnNodeCompleted?.Invoke();
	
	public static event UnityAction OnNextNodeSelected;
	public static void NextNodeSelected() => OnNextNodeSelected?.Invoke();
}