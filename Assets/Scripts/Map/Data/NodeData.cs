using System.Collections.Generic;
using UnityEngine;

public class NodeData {
	// 맵 데이터
	public MapNodeConfig Config { get; set; }
	// 렌더링될 위치
	public Vector2 Position { get; set; } = Vector2.zero;
	// 방문 여부
	public bool Visited { get; set; }
	// 내가 갈 수 있는 다음 위치
	public List<EdgeData> NextNodeIndices { get; } = new();
}