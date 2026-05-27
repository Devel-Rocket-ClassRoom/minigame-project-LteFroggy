using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameMap/MapGeneratingConfig")]
public class MapGeneratingConfig : ScriptableObject {
	// 가중치 풀 - 일반 노드 랜덤 선택에 사용
	[SerializeField] public List<MapNodeConfig> NodePool;

	public int LayerCount = 8;
	public int NodesPerLayer = 5;
	
	public float yStart = -443f;
	public float yDist = 150f;
	
	public float xStart = -844f;
	public float xDist = -572f - 844f;
	
	public float NodeExistChance = 0.6f;
	public float EdgeConnectChange = 0.6f;
	
	public Vector2 GetPosition(int row, int col) {
		return new Vector2(xStart + row * xDist, yStart + col * yDist);
	}

	public MapNodeConfig GetConfig(MapNodeType type)
	{
		foreach (var nodeConfig in NodePool) {
			if (nodeConfig.Type == type) return nodeConfig;
		}

		throw new ArgumentException($"Node config not found: {type}");
	}
}
