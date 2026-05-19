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

	[Header("=== 대상이 필요한 카드 선택 시 사용 ===")]
	[SerializeField] private CardLineDrawer _cardLineDrawer;
	
	private InputAction _clickAction;
	private InputAction _dragAction;
	private InputAction _positionAction;
	private Camera _mainCamera;
	private CardOnHandController _selectedCard;
	
	private bool _needsTarget;
	private Vector3 _lineStartPoint;
	// 타겟팅 필요 여부에 따라 LineDrawer 활성화, 비활성화 및 StartPoint 초기화
	public bool NeedsTarget {
		get => _needsTarget;
		set {
			_needsTarget = value;
			if (value) {
				_lineStartPoint = _mainCamera.ScreenToWorldPoint( 
					_positionAction.ReadValue<Vector2>());
				_cardLineDrawer.Show();
			} else {
				_lineStartPoint = Vector3.zero;
				_cardLineDrawer.Hide();
			}
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
		_dragAction = _inputAction.FindAction("Mouse/Drag");
		_positionAction = _inputAction.FindAction("Mouse/Position");
		_mainCamera = Camera.main;
	}

	private void OnEnable() {
		_clickAction.performed += OnClickStarted;
		_clickAction.canceled += OnClickReleased;
		
		_clickAction.Enable();
		_dragAction.Enable();
		_positionAction.Enable();
	}

	private void OnDisable() {
		_clickAction.performed -= OnClickStarted;
		_clickAction.canceled -= OnClickReleased;
		
		_clickAction.Disable();
		_dragAction.Disable();
		_positionAction.Disable();
	}

	// 클릭한 위치가 UI의 카드 위인지 검사
	private void OnClickStarted(InputAction.CallbackContext context) {
		Debug.Log($"클릭됨");
		
		// 현재 Position을 읽어서 position에 저장
		PointerEventData pointerData = new PointerEventData(EventSystem.current) {
			position = _positionAction.ReadValue<Vector2>()
		};
		
		// Position 기반으로 레이캐스트
		List<RaycastResult> results = new List<RaycastResult>();
		_graphicRaycaster.Raycast(pointerData, results);
		
		foreach (RaycastResult result in results) {
			CardOnHandController cardController = result.gameObject.GetComponent<CardOnHandController>();
			// 결과 중 카드가 있는지 검사
			if (cardController == null) { continue; }

			// 있다면 등록, 타겟이 필요한 카드인지도 검사해서 저장
			Debug.Log($"{cardController.CardIdxInHand}번 카드 클릭됨");
			_selectedCard = cardController;
			cardController.IsSelected = true;
			NeedsTarget = cardController.CardInstance._cardDefinition.needsTarget;
			break;
		}
	}
	
	private void OnClickReleased(InputAction.CallbackContext context) {
		if (_selectedCard == null) { return; }
		
		// 클릭된 카드 정보 초기화 및 제자리로 돌리기
		NeedsTarget = false;
		_selectedCard.IsSelected = false;
		_selectedCard.ToOriginalPosition();
		_selectedCard = null;
	}

	private void Update() {
		if (_selectedCard != null) {
			// 선택된 카드가 대상이 필요 없으면, 그냥 끌고 다닐 수 있어야 함
			if (!NeedsTarget) {
				_selectedCard.transform.position = _positionAction.ReadValue<Vector2>();	
			}
			
			// 대상이 필요하다면, 적을 선택할 수 있도록 화살표 표시
			if (NeedsTarget) {
				// 적이 선택되었는지 확인하기 위해 Ray 쏘기
				Vector2 mousePos = _mainCamera.ScreenToWorldPoint(_positionAction.ReadValue<Vector2>());
				RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, Layers.EnemyLayerMask);
				
				// 결과가 있다면, 화살표 거기로 붙게
				if (hit.collider !=null) {
					EnemyInstance enemyInstance = hit.collider.gameObject.GetComponent<EnemyInstance>();
					TargetInstance = enemyInstance;
					_cardLineDrawer.DrawTargetLine(
						_lineStartPoint,
						TargetInstance.transform.position);					
				}
				// 선택되지 않았다면, 마우스 따라오게
				else {
					_cardLineDrawer.DrawTargetLine(
						_lineStartPoint,
						_mainCamera.ScreenToWorldPoint(_positionAction.ReadValue<Vector2>())
					);
					TargetInstance = null;
				}
				
			}
		}
	}
}