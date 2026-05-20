using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardOnHandController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	private RectTransform _rectTransform;
	private RectTransform _drawPileLocation;
	private RectTransform _discardPileLocation;
	
	// 카드 위치 이동시킬 때 소요될 시간
	private readonly float _cardMoveDuration= 0.2f;
	private Coroutine _cardMoveCoroutine;

	// 마우스 올렸을 때 사용할 값
	private const float k_HoverY = 215f;
	private const float k_UseY = 525f;
	
	// 카드가 선택 상태가 되었을 때 이동할 위치
	private readonly Vector3 _selectedPosition = new Vector3(0, k_HoverY, 0);
	
	// 카드가 사용되었을 때 이동할 위치
	private readonly Vector3 _usePosition = new Vector3(0, k_UseY, 0);

	// 마우스 올렸다 뗐을 때 돌아오기 위한 변수들
	[SerializeField] private Vector3 _fanPosition;
	[SerializeField] private Quaternion _fanRotation;
	
	// Init시에 필요한 정보 세팅하기 위해 사용
	private CardInstance _cardInstance;
	public CardInstance CardInstance => _cardInstance;
	
	// 제자리로 돌아올 때 Sibling 사이 위치 결정하기 위해 사용
	public int CardIdxInHand { get; set; }

	[Header("=== 세팅할 정보 UI ===")]
	[SerializeField] private Image _cardIcon;
	[SerializeField] private TextMeshProUGUI _cardNameText;
	[SerializeField] private TextMeshProUGUI _cardDescriptionText;
	[SerializeField] private TextMeshProUGUI _cardCostText;

	[Header("=== 카드 사용, 삭제 시 이동할 위치 구하기 위함 ===")]
	[SerializeField] private RectTransform _drawPile;
	[SerializeField] private RectTransform _discardPile;
	
	// 본인 직접 삭제 시 카드풀에 반환하기 위함.
	private CardPool _cardPool;
	
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
	}
	
	public void Init(CardInstance instance, RectTransform drawPile, RectTransform discardPile, CardPool cardPool) {
		// Instance 세팅 후, 정보 넣기
		_cardInstance = instance;
		
		_isSelected = false;
		_cardIcon.sprite = instance._cardDefinition.icon;
		_cardNameText.text = instance._cardDefinition.CardName;
		_cardDescriptionText.text = instance.GetCardDescription();
		_cardCostText.text = instance._cardDefinition.cost.ToString();
		
		_drawPileLocation = drawPile;
		_discardPileLocation = discardPile;
		_cardPool = cardPool;
		
		// 시작 위치를 DrawPile쪽으로 해서 생성되면 그쪽에서 오는 것처럼 표현
		_rectTransform.position = _drawPileLocation.position;
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
		// 지금 선택된 카드는 Hover상태여도 HoverPosition으로 가지 않음
		if (_isSelected) { return; }
		ToHoverPosition();
	}

	public void OnPointerExit(PointerEventData eventData) {
		// 지금 선택된 카드는 커서 벗어나도 제자리로 돌아가지 않음
		if (_isSelected) {
			// 타겟이 필요한 카드는 선택 위치로 이동
			if (_cardInstance.NeedsTarget) { ToSelectedPosition(); }
			return;
		}
		
		ToOriginalPosition();
	}
	
	/// <summary>
	/// 카드에 마우스를 올렸을 때, 카드 내용이 다 보이도록 카드를 이동시킨다.
	/// </summary>
	private void ToHoverPosition() {
		_rectTransform.SetAsLastSibling();
		SetCardPosition(new Vector3(_fanPosition.x, k_HoverY, 0), Quaternion.identity);
	}
	
	/// <summary>
	/// 선택된 카드 위치로 카드를 이동
	/// </summary>
	private void ToSelectedPosition() {
		SetCardPosition(_selectedPosition, Quaternion.identity);
	}
	
	public void ToUsePosition() {
		SetCardPosition(_usePosition, Quaternion.identity);
	}
	
	/// <summary>
	/// 카드의 위치를 원래 위치로 되돌린다.
	/// </summary>
	public void ToOriginalPosition() {
		_rectTransform.SetSiblingIndex(CardIdxInHand);
		SetCardPosition(_fanPosition, _fanRotation);
	}
	
	/// <summary>
	/// 카드를 _discardPile의 위치로 이동시켜 제거한다.
	/// </summary>
	public void RemoveCard() {
		StartCoroutine(CoCardRemove());
	}
	
	/// <summary>
	/// 특정 위치로 카드가 이동할 때, 이동 애니메이션을 위한 함수
	/// </summary>
	/// <param name="targetPos">목표 위치</param>
	/// <param name="targetRot">목표 회전</param>
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
	
	/// <summary>
	/// 카드 제거할 때 사용, _discardPile의 위치로 이동한 후에 사라진다.
	/// </summary>
	/// <returns></returns>
	public IEnumerator CoCardRemove() {
		yield return CoCardMove(_discardPileLocation.position, Quaternion.identity);
		_cardPool.ReturnCard(this);
	}
}
