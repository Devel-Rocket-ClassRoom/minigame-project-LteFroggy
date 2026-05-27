using System.Collections.Generic;

public class InGameMapData {
	public NodeData[,] Nodes { get; set; }
	public List<NodeData> VisitedNodes { get; } = new();
	public NodeData NodeNow { get; set; }
	
	public InGameMapData() {
		Init();
		// 처음에 만들었을 때, 시작 지점은 항상 StartNode로
		NodeNow = Nodes[0, 2];
	}

	private void Init() {
		Nodes = MapGenerator.GenerateMap();
	}
}
