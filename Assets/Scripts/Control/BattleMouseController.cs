using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 전투 중 마우스 컨트롤 담당할 것
/// </summary>
public class BattleMouseController : MonoBehaviour {
	[Header("=== Input Manager 등록 ===")]
	[SerializeField] private InputActionAsset _inputAction;

	[Header("=== Canvas 등록 (카드와 Raycast 용도) ===")]
	[SerializeField] private GraphicRaycaster _graphicRaycaster;

	[Header("=== 대상이 필요한 카드 선택 시 대상 선택을 보여주는 라인 ===")]
	[SerializeField] private CardLineDrawer _cardLineDrawer;

	[Header("=== 카드 사용 가능한지 확인할 영역 ===")]
	[SerializeField] private RectTransform _cardUseArea;

	[Header("=== BattleManager ===")] 
	[SerializeField] private BattleManager _battleManager;
	
	// 클릭, 드래그,
	private InputAction _clickAction;
	private InputAction _mousePositionAction;
	private Camera _mainCamera;
	// 선택된 카드
	private CardOnHandController _selectedCard;
	
	// 대상 선택해야 할 상황이라면, 
	private RectTransform _lineStartPoint;
	
	private bool _needsTarget;
	// 타겟팅 필요 여부에 따라 LineDrawer 활성화, 비활성화
	public bool NeedsTarget {
		get => _needsTarget;
		set {
			_needsTarget = value;
			if (value) { _cardLineDrawer.Show(); } 
			else { _cardLineDrawer.Hide(); }
		}
	}
	
	private EnemyInstance _targetInstance;
	public EnemyInstance TargetInstance {
		get => _targetInstance;
		set {
			// 타겟이 생길 때, 하이라이트 박스 쳐주기
			if (value != null && _targetInstance == null) {
				value.SetTargetHighlight(true);
			}
			
			// 타겟이 사라질 때, 하이라이트 박스 없애주기
			if (_targetInstance != null && value == null) {
				_targetInstance.SetTargetHighlight(false);
			}
			_targetInstance = value;
		}
	}

	private void Awake() {
		_clickAction = _inputAction.FindAction("Mouse/Click");
		_mousePositionAction = _inputAction.FindAction("Mouse/Position");
		_mainCamera = Camera.main;
	}

	private void OnEnable() {
		_clickAction.performed += OnClickStarted;
		_clickAction.canceled += OnClickReleased;
		
		_clickAction.Enable();
		_mousePositionAction.Enable();
	}

	private void OnDisable() {
		_clickAction.performed -= OnClickStarted;
		_clickAction.canceled -= OnClickReleased;
		
		_clickAction.Disable();
		_mousePositionAction.Disable();
	}

	// 클릭한 위치가 UI의 카드 위인지 검사
	private void OnClickStarted(InputAction.CallbackContext context) {
		// 현재 Position을 읽어서 저장
		PointerEventData pointerData = new PointerEventData(EventSystem.current) {
			position = _mousePositionAction.ReadValue<Vector2>()
		};
		
		// Position 기반으로 레이캐스트
		List<RaycastResult> results = new List<RaycastResult>();
		_graphicRaycaster.Raycast(pointerData, results);
		
		foreach (RaycastResult result in results) {
			CardOnHandController cardController = result.gameObject.GetComponent<CardOnHandController>();
			// 결과 중 카드가 있는지 검사
			if (cardController == null) { continue; }

			// 있다면 등록, 타겟이 필요한 카드인지도 검사해서 저장
			_selectedCard = cardController;
			_lineStartPoint = cardController.GetComponent<RectTransform>();
			cardController.IsSelected = true;
			NeedsTarget = cardController.CardInstance._cardDefinition.needsTarget;
			break;
		}
	}
	
	private void OnClickReleased(InputAction.CallbackContext context) {
		if (_selectedCard == null) { return; }
		
		// 현재 충돌 위치가 CardUseArea 내부라면, 사용 처리
		if (RectTransformUtility.RectangleContainsScreenPoint(_cardUseArea, _mousePositionAction.ReadValue<Vector2>())) {
			// 사용 가능 여부등은 BattleManager에서 판단할 것. 여기선 사용 처리만
			if (!_battleManager.UseCard(_selectedCard.CardInstance, TargetInstance)) {
				// 사용 실패 시, 카드 상태까지 초기화
				DeselectSelectedCard();
			}
			ClearCardUseState();
			return;
		}
		// 사용 아닌 영역에서 내려뒀다면, 정보 초기화 및 카드 상태 초기화
		DeselectSelectedCard();
		ClearCardUseState();
	}
	
	public void DeselectSelectedCard() {
		_selectedCard.IsSelected = false;
		_selectedCard.ToOriginalPosition();
	}
	
	public void ClearCardUseState() {
		// 클릭된 카드 정보 초기화 및 제자리로 돌리기
		NeedsTarget = false;
		_selectedCard = null;
		_lineStartPoint = null;
		
		// 대상이 선택되었었다면, 대상 지정 UI 없애기
		if (TargetInstance != null) { TargetInstance.SetTargetHighlight(false); }
	}

	private void Update() {
		if (_selectedCard != null) {
			// 선택된 카드가 대상이 필요 없으면, 그냥 끌고 다닐 수 있어야 함
			if (!NeedsTarget) {
				_selectedCard.transform.position = _mousePositionAction.ReadValue<Vector2>();	
			}
			
			// 대상이 필요하다면, 적을 선택할 수 있도록 화살표 표시
			if (NeedsTarget) {
				// 적이 선택되었는지 확인하기 위해 Ray 쏘기
				Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_mousePositionAction.ReadValue<Vector2>());
				RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, Layers.EnemyLayerMask);
				
				// 결과가 있다면, 화살표 거기로 붙게
				if (hit.collider !=null) {
					EnemyInstance enemyInstance = hit.collider.gameObject.GetComponent<EnemyInstance>();
					TargetInstance = enemyInstance;
					_cardLineDrawer.DrawTargetLine(
						_mainCamera.ScreenToWorldPoint(_lineStartPoint.position),
						TargetInstance.transform.position);					
				}
				// 선택되지 않았다면, 마우스 따라오게
				else {
					_cardLineDrawer.DrawTargetLine(
						_mainCamera.ScreenToWorldPoint(_lineStartPoint.position),
						_mainCamera.ScreenToWorldPoint(_mousePositionAction.ReadValue<Vector2>())
					);
					TargetInstance = null;
				}
				
			}
		}
	}
}