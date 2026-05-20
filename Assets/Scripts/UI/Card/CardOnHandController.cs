using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardOnHandController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	private RectTransform _rectTransform;
	
	// 카드 위치 이동시킬 때 소요될 시간
	private readonly float _cardMoveDuration= 0.2f;
	private Coroutine _cardMoveCoroutine;

	// 마우스 올렸을 때 사용할 값
	private readonly float _hoverY = 264f;
	private readonly Vector3 _hoverScale = new (1.5f, 1.5f, 1.5f);

	// 마우스 올렸다 뗐을 때 돌아오기 위한 변수들
	private Vector3 _originalScale;
	[SerializeField] private Vector3 _fanPosition;
	[SerializeField] private Quaternion _fanRotation;
	private int _originalIndex;
	
	// Init시에 필요한 정보 세팅하기 위해 사용
	private CardInstance _cardInstance;
	public CardInstance CardInstance => _cardInstance;
	public int CardIdxInHand { get; set; }

	[Header("=== 세팅할 정보 UI ===")]
	[SerializeField] private Image _cardIcon;
	[SerializeField] private TextMeshProUGUI _cardNameText;
	[SerializeField] private TextMeshProUGUI _cardDescriptionText;
	[SerializeField] private TextMeshProUGUI _cardCostText;
	
	public void SetCardPosition(Vector3 location, Quaternion rotation) {
		if (_cardMoveCoroutine != null) {
			StopCoroutine(_cardMoveCoroutine);
			_cardMoveCoroutine = null;
		}
		_cardMoveCoroutine = StartCoroutine(CoCardMove(location, rotation));
	}
	
	private bool _isSelected;
	public bool IsSelected {
		get => _isSelected;
		set => _isSelected = value;
	}

	private void Awake() {
		_rectTransform = GetComponent<RectTransform>();
		_originalScale = transform.localScale;
	}
	
	public void Init(CardInstance instance, int cardIdxInHand) {
		// Instance 세팅 후, 정보 넣기
		_cardInstance = instance;
		
		_isSelected = false;
		CardIdxInHand = cardIdxInHand;
		_cardIcon.sprite = instance._cardDefinition.icon;
		_cardNameText.text = instance._cardDefinition.CardName;
		_cardDescriptionText.text = instance.GetCardDescription();
		_cardCostText.text = instance._cardDefinition.cost.ToString();
	}

	// 팬포지션 이동 시에 자연스럽게 움직이도록 하기 위함.
	public void SetFanTransform(Vector3 position, Quaternion rotation) {
		_fanPosition = position;
		_fanRotation = rotation;
		if (!_isSelected) {
			SetCardPosition(_fanPosition, _fanRotation);
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		ToHoverPosition();
	}

	public void OnPointerExit(PointerEventData eventData) {
		ToOriginalPosition();
	}
	
	private void ToHoverPosition() {
		_originalIndex = transform.GetSiblingIndex();

		_rectTransform.SetAsLastSibling();
		_rectTransform.localScale = _hoverScale;
		SetCardPosition(new Vector3(_fanPosition.x, _hoverY, 0), Quaternion.identity);
	}
	
	public void ToOriginalPosition() {
		_rectTransform.SetSiblingIndex(_originalIndex);
		_rectTransform.localScale = _originalScale;
		SetCardPosition(_fanPosition, _fanRotation);
	}
	
	public IEnumerator CoCardMove(Vector3 targetPos, Quaternion targetRot) {
		Vector3 startPos = _rectTransform.localPosition;
		Quaternion startRot = _rectTransform.localRotation;
		
		float timer = 0f;
		while (timer <= _cardMoveDuration) {
			timer += Time.deltaTime;
			yield return null;
			_rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, timer / _cardMoveDuration);
			_rectTransform.localRotation = Quaternion.Lerp(startRot, targetRot, timer / _cardMoveDuration);
		}
		
		_rectTransform.localPosition = targetPos;
		_rectTransform.localRotation = targetRot;
	}
}
