using UnityEngine;

public static class Layers {
	public static readonly int EnemyLayer = LayerMask.NameToLayer("Enemy");
	public static readonly int EnemyLayerMask = LayerMask.GetMask("Enemy");
}