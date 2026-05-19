using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour {

	[Header("=== 실제 UI카드상에 보이는 카드를 관리할 Controller ===")]
	[SerializeField] private HandLayoutController _handLayoutController;
	
	private readonly List<CardInstance> _drawPile = new();
	private readonly List<CardInstance> _discardPile = new();
	private readonly List<CardInstance> _exhaustPile = new();
	private readonly List<CardInstance> _handPile = new();
	
	
	/// <summary>
	/// 카드 한 장 드로우
	/// </summary>
	public void DrawCard() {
		if (_drawPile.Count == 0) { Shuffle(); }
		_handPile.Add(_drawPile[_drawPile.Count - 1]);
		_drawPile.RemoveAt(_drawPile.Count - 1);
		_handLayoutController.AddCard(_handPile[_handPile.Count - 1]);
	}
	
	public void RemoveCardFromHand(CardInstance card) { 
		_discardPile.Add(card);
		_handPile.Remove(card);
	}
	
	/// <summary>
	/// 카드 부족할 시, 카드를 섞는다.
	/// </summary>
	private void Shuffle() {
		_drawPile.Clear();
		
		List<List<int>> temp = new List<List<int>>(_discardPile.Count);
		
		// 정렬 기준으로 0 ~ 100까지의 값 랜덤 할당
		for (int i = 0; i < temp.Count; i++) {
			temp[i] = new List<int>();
			temp[i].Add(i);
			temp[i].Add(Random.Range(0, 100));
		}
		// 정렬 기준값 기반으로 Sort
		temp.Sort((listA, listB) => listA[1].CompareTo(listB[1]));
		
		// 이 기준으로 _drawPile에 삽입
		foreach (var sorted in temp) {
			_drawPile.Add(_discardPile[sorted[0]]);
		}
		// 버려진 카드 목록 초기화
		_discardPile.Clear();
	}
}