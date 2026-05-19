using UnityEngine;

public static class Layers {
	public static readonly int CardLayer = LayerMask.NameToLayer("Card");
	public static readonly int CardLayerMask =  LayerMask.GetMask("Card");
}