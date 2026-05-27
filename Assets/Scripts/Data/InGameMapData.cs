using System.Collections.Generic;

public class InGameMapData {
	public NodeData[,] Nodes { get; set; }
	public List<NodeData> VisitedNodes { get; } = new();
	
	public InGameMapData() {
		Init();
	}

	private void Init() {
		Nodes = MapGenerator.GenerateMap();
	}
}
