using UnityEngine;
using UnityEngine.UI;

public class NodeInstance : MonoBehaviour {

	[Header("=== 사용한 노드일 경우 보여줄 동그라미 표시 ===")]
	[SerializeField] private Image _circleImage;
	
	private NodeData _nodeData;
	private RectTransform _rectTransform;
	private Image _image;
	
	public void Init(NodeData data) {
		_rectTransform = GetComponent<RectTransform>();
		_image = GetComponent<Image>();
		_nodeData = data;
		
		// 자기 자신 배치하기
		DrawSelf();
		_circleImage.enabled = false;
	}

	private void DrawSelf() {
		// 이미지 설정
		_image.sprite = _nodeData.Config.Icon;
		// 필요한 위치에 배치
		_rectTransform.anchoredPosition = _nodeData.Position;
	}
	
	public void OnClick() {
		_nodeData.Visited = true;
		_circleImage.enabled = true;
	}
}