using UnityEngine;

public class CharacterManager : BattleSystemManager {
	
	[Header("=== 생성할 플레이어 캐릭터의 Prefab ===")]
	[SerializeField] private PlayerCharacter _playerPrefab;
	
	private PlayerCharacter _player;
	public PlayerCharacter Player => _player;
	
	private readonly Vector3 playerSpawnPoint = new Vector3(-6.2f, 0f, 0f);
	
	public override void StartBattle() {
		// 캐릭터 새로 만들고, 생성
		_player = Instantiate(_playerPrefab);
		_player.Init();
		_player.transform.position = playerSpawnPoint;
	}

	public override void StartPlayerTurn() {
		base.StartPlayerTurn();
		_player.OnTurnStart();
	}
	
	public override void EndPlayerTurn() {
		base.EndPlayerTurn();
		_player.OnTurnEnd();
	}
}