using UnityEngine;
using UnityEngine.UI;

public class EdgeInstance : MonoBehaviour {
	public EdgeData EdgeData { get; private set; }
	private Image _image;
	
	public void Init(NodeData start, EdgeData data) {
		_image = GetComponent<Image>();
		EdgeData = data;
		
		DrawEdge(start.Position);
	}
	
	private void DrawEdge(Vector2 start) {
		// 시작점과 끝점 설정
		Vector2 end = EdgeData.end.Position;
		_image.rectTransform.anchoredPosition = new Vector2((start.x + end.x) / 2f, (start.y + end.y) / 2f);
		_image.rectTransform.sizeDelta = new Vector2((end - start).magnitude, 2f);
		
		// 회전: start에서 end를 바라보는 각도
		float angle = Mathf.Atan2((end - start).y, (end - start).x) * Mathf.Rad2Deg;
		_image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
	}
}