using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class MapGenerator {
	public static MapGeneratingConfig MapConfig { get; set; }
	
	/// <summary>
	/// 가중치 기반으로 일반 노드 선택
	/// </summary>
	private static MapNodeConfig Pick() {
		int total = 0;
		foreach (var nodeConfig in MapConfig.NodePool) { total += nodeConfig.Weight; }
		int roll = Random.Range(0, total);
		int cumulative = 0;
		foreach (var nodeConfig in MapConfig.NodePool) {
			cumulative += nodeConfig.Weight;
			if (roll < cumulative) return nodeConfig;
		}
		throw new InvalidOperationException($"MapGenerator.Pick() : 랜덤값 {roll}이 적절하지 않습니다.");
	}
	
	public static NodeData[,] GenerateMap() {
		NodeData[,] nodes = new NodeData[MapConfig.LayerCount, MapConfig.NodesPerLayer];
		List<EdgeData> edges = new();
		int centerNodeIndex = MapConfig.NodesPerLayer / 2;
		
		// 첫 열은 노드가 중간에 하나만, 첫 열 노드는 방문하고 시작한걸로.
		nodes[0, centerNodeIndex] = new NodeData() {
			Config = MapConfig.GetConfig(MapNodeType.Start),
			Position = MapConfig.GetPosition(0, centerNodeIndex),
			Visited = true,
		};
		
		// 중간 열들은 모두 생성
		for (int i = 1; i < MapConfig.LayerCount - 1; i++) {
			for (int j = 0; j < MapConfig.NodesPerLayer; j++) {
				nodes[i, j] = new NodeData() {
					Config = Pick(),
					Position = MapConfig.GetPosition(i, j),
					Visited = false	
				};
			}
		}
		
		// 마지막 열은 노드가 중간에 하나만, 보스노드
		nodes[MapConfig.LayerCount - 1, centerNodeIndex] = new NodeData() {
			Config = MapConfig.GetConfig(MapNodeType.Boss),
			Position = MapConfig.GetPosition(MapConfig.LayerCount - 1, centerNodeIndex),
			Visited = false
		};
		
		// 노드 이어주기
		// 첫 Layer는 다음 레이어의 모든 값으로 갈 수 있음
		for (int i = 0; i < MapConfig.NodesPerLayer; i++) {
			nodes[0, centerNodeIndex].NextNodeIndices.Add(new EdgeData() {
				end = nodes[1, i],
				used = false
			});
		}
		
		// 중간 레이어들은 자기 바로 앞 값이랑만 이어지게
		for (int i = 1; i < MapConfig.LayerCount - 2; i++) {
			for (int j = 0; j < MapConfig.NodesPerLayer; j++) {
				nodes[i, j].NextNodeIndices.Add(new EdgeData() {
					end = nodes[i + 1, j],
					used = false
				});
			}
		}
		
		// 마지막에서 두 번째 레이어는, 마지막 값이랑 모두 이어진다
		for (int i = 0; i < MapConfig.NodesPerLayer; i++) {
			nodes[MapConfig.LayerCount - 2, i].NextNodeIndices.Add(new EdgeData() {
				end = nodes[MapConfig.LayerCount - 1, centerNodeIndex],
				used = false
			});
		}
		
		return nodes;
	}
	
	/* 일단 시간이 너무 오래 소요되어, 대충 처리 */
	// // 맵 전체 생성하기 (마지막은 보스, 그 전까진 랜덤값에 따라)
	// public static void GenerateMap() {
	// 	NodeData[,] nodes = new NodeData[MapConfig.LayerCount, MapConfig.NodesPerLayer];
	//
	// 	// 마지막 열부터 역순으로 생성
	// 	// 마지막 열은 노드가 중간에 하나만, 보스노드
	// 	nodes[MapConfig.LayerCount - 1, MapConfig.NodesPerLayer / 2] = new NodeData() {
	// 		Config = MapConfig.GetConfig(MapNodeType.Boss),
	// 		Visited = false
	// 	};
	// 	
	// 	// 마지막에서 두 번째 열
	// 	for (int i = 0; i < MapConfig.NodesPerLayer; i++) {
	// 		// 생성 확률 넘으면 생성, 못넘었으면 null로 놔두기
	// 		if (Random.Range(0, 1) < MapConfig.NodeExistChance) {
	// 			var node = nodes[MapConfig.LayerCount - 2, i] = new NodeData() {
	// 				Config = Pick(),
	// 				Visited = false,
	// 			};
	// 			
	// 			// 마지막에서 두 번째 열은 항상 보스로 갈 수 있어야 함
	// 			node.NextNodeIndices.Add((MapConfig.LayerCount - 1, 2));
	// 			nodes[MapConfig.LayerCount - 1, i].PreviousNodeCount++;
	// 		}
	// 	}
	// 	
	// 	// 마지막에서 세 번째 열부터 첫 열까지
	// 	for (int i = MapConfig.LayerCount - 3; i > 0; i--) {
	// 		for (int j = 0; j < MapConfig.NodesPerLayer - 1; j++) {
	// 			// 현재 노드가 생성이 가능한지 확인 => 내 위 3개의 노드 체크
	// 			bool[] isUpperNodeExists = new bool[3];
	// 			int upperNodeCount = 0;
	// 			for (int k = -1; k < 2; k++) {
	// 				// index를 넘어가면, 없는 것 처리
	// 				if (j + k < 0 || j + k >= MapConfig.NodesPerLayer) {
	// 					isUpperNodeExists[k + 1] = false;
	// 					continue;
	// 				} if (nodes[i + 1,j + k] != null) {
	// 					isUpperNodeExists[k + 1] = true;
	// 					upperNodeCount++;
	// 				}
	// 			}
	// 			
	// 			// 내가 이어질 노드가 하나도 없다면, 생성 안함
	// 			if (upperNodeCount == 0) continue;
	// 			
	// 			// 내가 이어져야 할 노드가 있다면, 필수인지 먼저 체크
	// 			if (Random.Range(0, 1) < MapConfig.NodeExistChance) {
	// 				var node = nodes[i, j] = new NodeData() {
	// 					Config = Pick(),
	// 					Visited = false,
	// 				};
	// 				
	// 				// 다음 노드로 가는 연결을 만들어야 한다.
	// 				// 1. 이어진 노드가 하나라면, 바로 그거랑만 이어주면 됨
	// 				if (upperNodeCount == 1) {
	// 					for (int k = 0; k < 3; k++) {
	// 						if (isUpperNodeExists[k]) {
	// 							node.NextNodeIndices.Add((i + 1, j + k - 1));
	// 							break;
	// 						}
	// 					}
	// 				} // 이어진 노드가 두 개 이상이라면, 랜덤 하나는 확정으로, 나머지는 확률로
	// 				else {
	// 					int 
	// 				}
	// 			}
	// 			
	// 			// 만약 마지막에서 두 번째 노드라면, 항상 보스 노드로 이어질 수 있어야 함.
	// 		}	
	// 	}
	//
	// 	// 첫 열은 노드가 중간에 하나만, 일단 전투노드
	// 	nodes[0,2] = new NodeData() {
	// 		Config = MapConfig.GetConfig(MapNodeType.Battle),
	// 		Visited = false
	// 	};
	//
	//
	// 	
	// 	mapData.nodes[MapConfig.LayerCount - 1] = new NodeData() { Config = MapConfig.GetConfig(MapNodeType.Boss) };
	// }
}
