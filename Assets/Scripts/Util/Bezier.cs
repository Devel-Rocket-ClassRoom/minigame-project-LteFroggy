using UnityEngine;

public static class Bezier {
	public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float u = 1f - t;

		return
			u * u * u * p0 +
			3f * u * u * t * p1 +
			3f * u * t * t * p2 +
			t * t * t * p3;
	}
}