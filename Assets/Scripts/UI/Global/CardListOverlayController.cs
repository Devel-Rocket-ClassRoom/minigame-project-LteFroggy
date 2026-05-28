using System.Collections.Generic;
using UnityEngine;

public class CardListOverlayController : MonoBehaviour {
	private static CardListOverlayController _instance;

	public static CardListOverlayController Instance {
		get {
			if (_instance != null) return _instance;
			return _instance = FindAnyObjectByType<CardListOverlayController>();
		}
	}

	[SerializeField] private OverlayPanelController _overlay;
	[SerializeField] private DeckRenderer _deckRenderer;

	private void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this);
			return;
		}

		_instance = this;
		CacheComponents();
	}

	private void OnDestroy() {
		if (_instance == this) _instance = null;
	}

	public void Show(IEnumerable<CardInstance> cards, string descriptionText) {
		CacheComponents();
		if (_overlay == null || _deckRenderer == null) return;

		_overlay.Open();
		_deckRenderer.Init(cards, descriptionText, null);
	}

	public void Toggle(IEnumerable<CardInstance> cards, string descriptionText) {
		CacheComponents();
		if (_overlay == null || _deckRenderer == null) return;

		if (_overlay.gameObject.activeSelf) {
			Close();
			return;
		}

		Show(cards, descriptionText);
	}

	public void Close() {
		CacheComponents();
		if (_overlay == null || _deckRenderer == null) return;

		_overlay.Close();
		_deckRenderer.Clear();
	}

	private void CacheComponents() {
		if (_overlay == null) _overlay = GetComponent<OverlayPanelController>();
		if (_deckRenderer == null) _deckRenderer = GetComponent<DeckRenderer>();
	}
}
