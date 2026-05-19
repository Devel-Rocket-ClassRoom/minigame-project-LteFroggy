using UnityEngine;

public class CardLineDrawer : MonoBehaviour {
	private LineRenderer _lineRenderer;
	[SerializeField] private int _resolution = 30;
	[SerializeField] private float _curveHeight = 2f;
	[SerializeField] private float _lineZ = -1f;
	
	private void Awake() {
		_lineRenderer = GetComponent<LineRenderer>();
		Hide();
	}
	
	// 그리기 시작
	public void Show() {
		_lineRenderer.enabled = true;
	}
	
	// 라인 그리기 끝나면 SetActive False
	public void Hide() {
		_lineRenderer.enabled = false;
	}
	
	public void DrawTargetLine(Vector3 startPoint, Vector3 endPoint) {
		startPoint.z = _lineZ;
		endPoint.z = _lineZ;

		Vector3 dir = endPoint - startPoint;

		Vector3 p0 = startPoint;
		Vector3 p3 = endPoint;

		Vector3 p1 = startPoint + dir * 0.25f + Vector3.up * _curveHeight;
		Vector3 p2 = startPoint + dir * 0.75f + Vector3.up * _curveHeight;

		_lineRenderer.positionCount = _resolution;

		for (int i = 0; i < _resolution; i++)
		{
			float t = (float)i / (_resolution - 1);
			Vector3 point = Bezier.GetBezierPoint(p0, p1, p2, p3, t);
			_lineRenderer.SetPosition(i, point);
		}
	}
}