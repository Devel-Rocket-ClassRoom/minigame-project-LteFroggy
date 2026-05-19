using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardOnHandController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	private RectTransform _rectTransform;

	// 마우스 올렸을 때 사용할 값
	private readonly float _hoverY = 264f;
	private readonly Vector3 _hoverScale = new (1.5f, 1.5f, 1.5f);

	// 마우스 올렸다 뗐을 때 돌아오기 위한 변수들
	private Vector3 _originalScale;
	private Vector3 _fanPosition;
	private Quaternion _fanRotation;
	private int _originalIndex;
	
	// Init시에 자신의 정보 직접 세팅하기 위해 사용
	private CardInstance _cardInstance;
	public CardInstance CardInstance;
	public int CardIdxInHand { get; set; }

	[Header("=== 세팅할 정보 UI ===")]
	[SerializeField] private Image _cardIcon;
	[SerializeField] private TextMeshProUGUI _cardNameText;
	[SerializeField] private TextMeshProUGUI _cardDescriptionText;
	[SerializeField] private TextMeshProUGUI _cardCostText;
	
	private bool _isFocused;

	private CardInstance _instance;

	private void Awake() {
		_rectTransform = GetComponent<RectTransform>();
		_originalScale = transform.localScale;
	}
	
	public void Init(CardInstance instance, int cardIdxInHand) {
		// Instance 세팅 후, 정보 넣기
		_instance = instance;
		
		CardIdxInHand = cardIdxInHand;
		_cardIcon.sprite = instance.cardDefinition.icon;
		_cardNameText.text = instance.cardDefinition.name;
		_cardDescriptionText.text = instance.GetCardDescription();
		_cardCostText.text = instance.cardDefinition.cost.ToString();
	}

	public void SetFanTransform(Vector3 position, Quaternion rotation) {
		_fanPosition = position;
		_fanRotation = rotation;
		if (!_isFocused) {
			_rectTransform.localPosition = position;
			_rectTransform.localRotation = rotation;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (_isFocused) {
			return;
		}

		_isFocused = true;
		_originalIndex = transform.GetSiblingIndex();

		_rectTransform.SetAsLastSibling();
		_rectTransform.localScale = _hoverScale;
		_rectTransform.localPosition = new Vector3(_fanPosition.x, _hoverY, 0);
		_rectTransform.localRotation = Quaternion.identity;
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (!_isFocused) {
			return;
		}

		_isFocused = false;
		_rectTransform.SetSiblingIndex(_originalIndex);
		_rectTransform.localScale = _originalScale;
		_rectTransform.localPosition = _fanPosition;
		_rectTransform.localRotation = _fanRotation;
	}
}
