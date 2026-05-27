using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour {
    // 선이 노드보다 뒤에 보이도록, 노드 부모보다 아래쪽 계층에 둔다.
    [SerializeField] private Transform _edgeParent;
    // 생성된 맵 노드 UI들이 들어갈 부모 Transform
    [SerializeField] private Transform _nodeParent;

    [Header("=== 생성할 노드의 Instance ===")]
    [SerializeField] private NodeInstance _nodeInstance;
    [SerializeField] private EdgeInstance _edgeInstance;
    private readonly Dictionary<NodeData, NodeInstance> _nodeMappingTable = new();
    private readonly Dictionary<NodeData, List<EdgeInstance>> _edgeMappingTable = new();
    
    private void Start() {
        // 맵 그리기
        BuildMap();
    }
    
    // 맵 데이터를 참조해서 맵을 그리고, 저장한다.
    private void BuildMap() {
        // NodeParent, EdgeParent 모두 청소
        ClearChildren(_nodeParent);
        ClearChildren(_edgeParent);
        
        // 노드 그리기
        _nodeMappingTable.Clear();
        foreach (var nodeData in GamePlayData.Instance.InGameMapData.Nodes) {
            if (nodeData == null) continue;
            
            var nodeInstance = Instantiate(_nodeInstance, _nodeParent);
            nodeInstance.Init(nodeData);
            _nodeMappingTable.Add(nodeData, nodeInstance);
            
            // 각 노드별로 Edge 그리기
            _edgeMappingTable[nodeData] = new List<EdgeInstance>();
            foreach (var edgeData in nodeData.NextNodeIndices) {
                var edgeInstance = Instantiate(_edgeInstance, _edgeParent);
                edgeInstance.Init(nodeData, edgeData);
                _edgeMappingTable[nodeData].Add(edgeInstance);
            }
        }
    }
    
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
