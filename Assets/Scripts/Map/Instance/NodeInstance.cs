using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NodeInstance : MonoBehaviour {

	[Header("=== 사용한 노드일 경우 보여줄 동그라미 표시 ===")]
	[SerializeField] private Image _circleImage;
	
	private MapRenderer _mapRenderer;
	private NodeData _nodeData;
	private RectTransform _rectTransform;
	private Image _image;
	private Button _button;
	
	public void Init(NodeData data, MapRenderer mapRenderer) {
		_rectTransform = GetComponent<RectTransform>();
		_image = GetComponent<Image>();
		_button = GetComponent<Button>();
		
		_nodeData = data;
		_mapRenderer = mapRenderer;
		
		// 자기 자신 배치하기
		DrawSelf();
		
		// 처음엔 클릭 비활성화
		DisableButton();
		
		_button.onClick.AddListener(OnClick);
	}
	
	public void DisableButton() {
		_image.color = new Color(1f, 1f, 1f, 0.5f);
		_button.interactable = false;
	}

	private void OnDestroy() {
		_button.onClick.RemoveAllListeners();
	}

	// 클릭되면, InGameMapData에 현재 노드 갱신해주고 전체 클릭 비활성화
	public void OnClick() {
		GamePlayData.Instance.InGameMapData.NodeNow = _nodeData;
		
		_nodeData.Visited = true;
		_circleImage.enabled = _nodeData.Visited;
		_mapRenderer.DisableAllSelectableNodes();
		
		GameEvents.NextNodeSelected();
		
		// 그리고 실제 내가 가진 데이터대로 씬 변경
		UISceneBootstrapper.Instance.TransitionTo(_nodeData.Config.SceneName);
	}

	public void EnableButton() {
		_image.color = new Color(1f, 1f, 1f, 1f);
		_button.interactable = true;
	}

	private void DrawSelf() {
		// 이미지 설정
		_image.sprite = _nodeData.Config.Icon;
		// 필요한 위치에 배치
		_rectTransform.anchoredPosition = _nodeData.Position;
		// 방문 여부에 따라 동그라미 처리
		_circleImage.enabled = _nodeData.Visited;
	}
}