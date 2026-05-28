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
    
    private readonly List<NodeInstance> _selectableInstances = new();

    public void Init() {
        // 맵 그리기
        BuildMap();
        
        // 맵 클리어 시, 렌더링 갱신하는 함수 추가.
        GameEvents.OnNodeCompleted += UpdateMapRendering;
    }

    private void OnDestroy() {
        GameEvents.OnNodeCompleted -= UpdateMapRendering;
    }

    private void UpdateMapRendering() {
        // 현재 어떤 노드에 존재하는지 확인
        NodeData clearedNode = GamePlayData.Instance.InGameMapData.NodeNow;
        
        // 이번 노드 클리어했으니, 다음 노드 상태 갱신
        foreach (var edge in _edgeMappingTable[clearedNode]) {
            // 이번 노드에서 이어진 다음 노드들, 전부 클릭 가능 상태로 만들기
            _nodeMappingTable[edge.EdgeData.end].EnableButton();
            // 클릭 가능 값들 저장해두기.
            _selectableInstances.Add(_nodeMappingTable[edge.EdgeData.end]);
        }
    }
    
    /// <summary>
    /// 현재 선택 가능(클릭 가능)상태인 모든 노드를 클릭 불가능하게 만든다.
    /// 여러 노드 중 하나가 선택되었을 때 사용
    /// </summary>
    public void DisableAllSelectableNodes() {
        while (_selectableInstances.Count > 0) {
            _selectableInstances[0].DisableButton();
            _selectableInstances.RemoveAt(0);
        }
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
            nodeInstance.Init(nodeData, this);
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
