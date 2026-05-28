using System;
using System.Collections.Generic;
using UnityEngine;



public class CardViewRenderer : MonoBehaviour {
	[Header("=== 생성된 카드 인스턴스가 보일 곳 ===")]
	[SerializeField] private Transform _cardParent;
	[SerializeField] private CardViewController _cardPrefab;
	private List<CardInstance> _cardInstances = GamePlayData.Instance.Deck;
	
	// 새로 띄워질 때마다 덱 리스트 렌더링
	private void OnEnable() {
		
	}
}