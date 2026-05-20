using System;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour {
	
	[Header("=== 생성할 플레이어 캐릭터의 Prefab ===")]
	[SerializeField] private PlayerCharacter _playerPrefab;
	
	private readonly Vector3 playerSpawnPoint = new Vector3(-4.2f, 0f, 0f);

	private void Start() {
		// 캐릭터 새로 만들고, 생성
		var playerObject = Instantiate(_playerPrefab);
		playerObject.transform.position = playerSpawnPoint;
	}
}