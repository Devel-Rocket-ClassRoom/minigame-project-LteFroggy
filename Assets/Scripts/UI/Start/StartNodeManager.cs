using System;
using UnityEngine;
using UnityEngine.UI;

public class StartNodeManager : MonoBehaviour {
	[Header("=== 다음 노드로 갈 버튼 ===")]
	[SerializeField] private Button _button;

	// 버튼 누르면 다음 노드로 가게 함.
	private void OnEnable() {
		_button.onClick.AddListener(GameEvents.NodeCompleted);
	}

	private void OnDisable() {
		_button.onClick.RemoveListener(GameEvents.NodeCompleted);
	}
}